locals {
  failover_enabled = var.num_cache_clusters > 1 ? var.automatic_failover_enabled : false
  multi_az_enabled = var.num_cache_clusters > 1 ? var.multi_az_enabled : false
}

resource "aws_security_group" "this" {
  name        = "${var.name}-sg"
  description = "Security group for ${var.name}"
  vpc_id      = var.vpc_id

  tags = merge(var.tags, {
    Name = "${var.name}-sg"
  })
}

resource "aws_vpc_security_group_ingress_rule" "app" {
  for_each = toset(var.allowed_security_group_ids)

  security_group_id            = aws_security_group.this.id
  referenced_security_group_id = each.value
  from_port                    = 6379
  to_port                      = 6379
  ip_protocol                  = "tcp"
  description                  = "Allow Redis access from application security groups"
}

resource "aws_vpc_security_group_egress_rule" "all" {
  security_group_id = aws_security_group.this.id
  cidr_ipv4         = "0.0.0.0/0"
  ip_protocol       = "-1"
  description       = "Allow outbound traffic"
}

resource "aws_elasticache_subnet_group" "this" {
  name       = "${var.name}-subnets"
  subnet_ids = var.subnet_ids

  tags = merge(var.tags, {
    Name = "${var.name}-subnets"
  })
}

resource "aws_elasticache_replication_group" "this" {
  replication_group_id       = var.name
  description                = "Redis replication group for ${var.name}"
  engine                     = "redis"
  engine_version             = var.engine_version
  node_type                  = var.node_type
  port                       = 6379
  parameter_group_name       = "default.redis7"
  subnet_group_name          = aws_elasticache_subnet_group.this.name
  security_group_ids         = [aws_security_group.this.id]
  automatic_failover_enabled = local.failover_enabled
  multi_az_enabled           = local.multi_az_enabled
  num_cache_clusters         = var.num_cache_clusters
  at_rest_encryption_enabled = true
  transit_encryption_enabled = false
  auto_minor_version_upgrade = true
  apply_immediately          = var.apply_immediately
  snapshot_retention_limit   = var.snapshot_retention_limit

  tags = merge(var.tags, {
    Name = var.name
  })
}
