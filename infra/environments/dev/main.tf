data "aws_caller_identity" "current" {}

locals {
  environment            = "dev"
  name_prefix            = "${var.project_name}-${local.environment}"
  container_name         = "wex-purchases-api"
  application_port       = 8080
  aspnetcore_environment = "Development"
  common_tags = {
    Application = "wex-purchases-api"
    Layer       = "platform"
  }
}

module "vpc" {
  source = "../../modules/vpc"

  name                      = local.name_prefix
  vpc_cidr                  = var.vpc_cidr
  az_count                  = 2
  public_subnet_cidrs       = var.public_subnet_cidrs
  private_app_subnet_cidrs  = var.private_app_subnet_cidrs
  private_data_subnet_cidrs = var.private_data_subnet_cidrs
  enable_nat_gateway        = true
  single_nat_gateway        = true
  tags                      = local.common_tags
}

module "ecr" {
  source = "../../modules/ecr"

  name            = "${var.project_name}-api-${local.environment}"
  max_image_count = 20
  tags            = local.common_tags
}

module "alb" {
  source = "../../modules/alb"

  name                       = "${local.name_prefix}-alb"
  vpc_id                     = module.vpc.vpc_id
  public_subnet_ids          = module.vpc.public_subnet_ids
  container_port             = local.application_port
  health_check_path          = "/health"
  allowed_cidrs              = var.allowed_ingress_cidrs
  certificate_arn            = var.alb_certificate_arn
  enable_deletion_protection = false
  alarm_actions              = var.alarm_actions
  tags                       = local.common_tags
}

resource "aws_security_group" "ecs_service" {
  name        = "${local.name_prefix}-ecs-sg"
  description = "Security group for ECS application tasks"
  vpc_id      = module.vpc.vpc_id

  tags = merge(local.common_tags, {
    Name = "${local.name_prefix}-ecs-sg"
  })
}

resource "aws_vpc_security_group_ingress_rule" "ecs_from_alb" {
  security_group_id            = aws_security_group.ecs_service.id
  referenced_security_group_id = module.alb.security_group_id
  from_port                    = local.application_port
  to_port                      = local.application_port
  ip_protocol                  = "tcp"
  description                  = "Allow ALB traffic to ECS tasks"
}

resource "aws_vpc_security_group_egress_rule" "ecs_all" {
  security_group_id = aws_security_group.ecs_service.id
  cidr_ipv4         = "0.0.0.0/0"
  ip_protocol       = "-1"
  description       = "Allow outbound traffic from ECS tasks"
}

module "rds_mysql" {
  source = "../../modules/rds-mysql"

  name                         = "${local.name_prefix}-mysql"
  vpc_id                       = module.vpc.vpc_id
  subnet_ids                   = module.vpc.private_data_subnet_ids
  allowed_security_group_ids   = [aws_security_group.ecs_service.id]
  db_name                      = var.db_name
  username                     = var.db_username
  instance_class               = var.db_instance_class
  allocated_storage            = var.db_allocated_storage
  max_allocated_storage        = var.db_max_allocated_storage
  backup_retention_period      = 7
  multi_az                     = false
  deletion_protection          = false
  skip_final_snapshot          = true
  apply_immediately            = true
  performance_insights_enabled = true
  alarm_actions                = var.alarm_actions
  tags                         = local.common_tags
}

module "redis" {
  source = "../../modules/elasticache-redis"

  name                       = "${local.name_prefix}-redis"
  vpc_id                     = module.vpc.vpc_id
  subnet_ids                 = module.vpc.private_data_subnet_ids
  allowed_security_group_ids = [aws_security_group.ecs_service.id]
  node_type                  = var.redis_node_type
  num_cache_clusters         = var.redis_num_cache_clusters
  apply_immediately          = true
  snapshot_retention_limit   = 1
  alarm_actions              = var.alarm_actions
  tags                       = local.common_tags
}

resource "aws_secretsmanager_secret" "purchase_db_connection" {
  name                    = "/${var.project_name}/${local.environment}/ConnectionStrings/PurchaseDb"
  recovery_window_in_days = 0

  tags = local.common_tags
}

resource "aws_secretsmanager_secret_version" "purchase_db_connection" {
  secret_id     = aws_secretsmanager_secret.purchase_db_connection.id
  secret_string = "server=${module.rds_mysql.address};port=${module.rds_mysql.port};database=${module.rds_mysql.db_name};user=${module.rds_mysql.username};password=${module.rds_mysql.password}"
}

resource "aws_secretsmanager_secret" "redis_connection" {
  name                    = "/${var.project_name}/${local.environment}/ConnectionStrings/Redis"
  recovery_window_in_days = 0

  tags = local.common_tags
}

resource "aws_secretsmanager_secret_version" "redis_connection" {
  secret_id     = aws_secretsmanager_secret.redis_connection.id
  secret_string = "${module.redis.primary_endpoint_address}:${module.redis.port}"
}

module "ecs" {
  source = "../../modules/ecs"

  name                              = local.name_prefix
  aws_region                        = var.aws_region
  subnet_ids                        = module.vpc.private_app_subnet_ids
  target_group_arn                  = module.alb.target_group_arn
  security_group_ids                = [aws_security_group.ecs_service.id]
  container_name                    = local.container_name
  container_port                    = local.application_port
  container_image                   = "${module.ecr.repository_url}:${var.container_image_tag}"
  task_cpu                          = var.ecs_task_cpu
  task_memory                       = var.ecs_task_memory
  desired_count                     = var.ecs_desired_count
  health_check_grace_period_seconds = 120
  log_retention_days                = 30
  environment_variables = {
    ASPNETCORE_ENVIRONMENT                = local.aspnetcore_environment
    ASPNETCORE_URLS                       = "http://+:8080"
    TreasuryApi__BaseUrl                  = "https://api.fiscaldata.treasury.gov"
    TreasuryApi__RatesOfExchangeEndpoint  = "/services/api/fiscal_service/v1/accounting/od/rates_of_exchange"
    TreasuryApi__TimeoutSeconds           = "10"
    TreasuryApi__HttpClientTimeoutSeconds = "100"
  }
  secret_arns = {
    ConnectionStrings__PurchaseDb = aws_secretsmanager_secret.purchase_db_connection.arn
    ConnectionStrings__Redis      = aws_secretsmanager_secret.redis_connection.arn
  }
  enable_autoscaling       = var.ecs_enable_autoscaling
  autoscaling_min_capacity = var.ecs_autoscaling_min_capacity
  autoscaling_max_capacity = var.ecs_autoscaling_max_capacity
  cpu_target_value         = 70
  memory_target_value      = 75
  alarm_actions            = var.alarm_actions
  tags                     = local.common_tags
}
