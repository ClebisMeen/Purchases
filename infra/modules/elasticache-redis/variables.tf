variable "name" {
  description = "Base name used for Redis resources."
  type        = string
}

variable "vpc_id" {
  description = "VPC where Redis is deployed."
  type        = string
}

variable "subnet_ids" {
  description = "Private data subnet IDs used by Redis."
  type        = list(string)
}

variable "allowed_security_group_ids" {
  description = "Security groups allowed to reach Redis."
  type        = list(string)
}

variable "node_type" {
  description = "ElastiCache node type."
  type        = string
}

variable "engine_version" {
  description = "Redis engine version."
  type        = string
  default     = "7.1"
}

variable "num_cache_clusters" {
  description = "Number of cache clusters in the replication group."
  type        = number
  default     = 1
}

variable "automatic_failover_enabled" {
  description = "Enable automatic failover when using multiple nodes."
  type        = bool
  default     = true
}

variable "multi_az_enabled" {
  description = "Enable Multi-AZ when using multiple nodes."
  type        = bool
  default     = true
}

variable "apply_immediately" {
  description = "Apply changes immediately."
  type        = bool
  default     = false
}

variable "snapshot_retention_limit" {
  description = "How many days Redis snapshots are retained."
  type        = number
  default     = 1
}

variable "alarm_actions" {
  description = "Alarm actions for Redis alarms."
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "Tags applied to Redis resources."
  type        = map(string)
  default     = {}
}
