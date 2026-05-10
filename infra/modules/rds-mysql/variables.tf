variable "name" {
  description = "Base name used for the RDS resources."
  type        = string
}

variable "vpc_id" {
  description = "VPC where RDS is deployed."
  type        = string
}

variable "subnet_ids" {
  description = "Private data subnet IDs for RDS."
  type        = list(string)
}

variable "allowed_security_group_ids" {
  description = "Security groups allowed to reach the database."
  type        = list(string)
}

variable "db_name" {
  description = "Initial database name."
  type        = string
}

variable "username" {
  description = "Master username."
  type        = string
}

variable "instance_class" {
  description = "RDS instance class."
  type        = string
}

variable "allocated_storage" {
  description = "Initial allocated storage in GiB."
  type        = number
}

variable "max_allocated_storage" {
  description = "Maximum autoscaled storage in GiB."
  type        = number
}

variable "engine_version" {
  description = "MySQL engine version."
  type        = string
  default     = "8.0"
}

variable "backup_retention_period" {
  description = "Automated backup retention in days."
  type        = number
  default     = 7
}

variable "multi_az" {
  description = "Whether the database is multi-AZ."
  type        = bool
  default     = false
}

variable "deletion_protection" {
  description = "Enable deletion protection."
  type        = bool
  default     = true
}

variable "skip_final_snapshot" {
  description = "Skip final snapshot on destroy."
  type        = bool
  default     = false
}

variable "apply_immediately" {
  description = "Apply changes immediately."
  type        = bool
  default     = false
}

variable "performance_insights_enabled" {
  description = "Enable Performance Insights."
  type        = bool
  default     = true
}

variable "alarm_actions" {
  description = "Alarm actions for RDS alarms."
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "Tags applied to RDS resources."
  type        = map(string)
  default     = {}
}
