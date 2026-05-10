variable "name" {
  description = "Base name used to tag and name VPC resources."
  type        = string
}

variable "vpc_cidr" {
  description = "Primary CIDR block for the VPC."
  type        = string
}

variable "az_count" {
  description = "Number of availability zones to use."
  type        = number
}

variable "public_subnet_cidrs" {
  description = "CIDRs for public subnets."
  type        = list(string)
}

variable "private_app_subnet_cidrs" {
  description = "CIDRs for private application subnets."
  type        = list(string)
}

variable "private_data_subnet_cidrs" {
  description = "CIDRs for private data subnets."
  type        = list(string)
}

variable "enable_nat_gateway" {
  description = "Whether to create NAT gateways."
  type        = bool
  default     = true
}

variable "single_nat_gateway" {
  description = "Whether to create a single shared NAT gateway."
  type        = bool
  default     = true
}

variable "tags" {
  description = "Tags applied to all VPC resources."
  type        = map(string)
  default     = {}
}
