variable "name" {
  description = "Base name used for the ALB resources."
  type        = string
}

variable "vpc_id" {
  description = "VPC where the ALB is deployed."
  type        = string
}

variable "public_subnet_ids" {
  description = "Public subnet IDs used by the ALB."
  type        = list(string)
}

variable "container_port" {
  description = "Application container port targeted by the ALB."
  type        = number
}

variable "health_check_path" {
  description = "Health check path for the target group."
  type        = string
  default     = "/health"
}

variable "allowed_cidrs" {
  description = "CIDR ranges allowed to reach the ALB."
  type        = list(string)
}

variable "certificate_arn" {
  description = "Optional ACM certificate ARN to enable HTTPS."
  type        = string
  default     = null
}

variable "idle_timeout" {
  description = "Idle timeout in seconds."
  type        = number
  default     = 60
}

variable "deregistration_delay" {
  description = "Target group deregistration delay in seconds."
  type        = number
  default     = 30
}

variable "enable_deletion_protection" {
  description = "Enable deletion protection on the ALB."
  type        = bool
  default     = false
}

variable "alarm_actions" {
  description = "Alarm actions for ALB-related CloudWatch alarms."
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "Tags applied to ALB resources."
  type        = map(string)
  default     = {}
}
