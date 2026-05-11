variable "name" {
  description = "Name of the ECR repository."
  type        = string
}

variable "image_tag_mutability" {
  description = "Whether image tags can be overwritten."
  type        = string
  default     = "MUTABLE"
}

variable "scan_on_push" {
  description = "Enable image scanning on push."
  type        = bool
  default     = true
}

variable "max_image_count" {
  description = "Maximum number of images to retain in the repository."
  type        = number
  default     = 30
}

variable "tags" {
  description = "Tags applied to the ECR repository."
  type        = map(string)
  default     = {}
}
