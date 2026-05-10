output "load_balancer_arn" {
  description = "ALB ARN."
  value       = aws_lb.this.arn
}

output "dns_name" {
  description = "ALB DNS name."
  value       = aws_lb.this.dns_name
}

output "zone_id" {
  description = "ALB Route53 zone identifier."
  value       = aws_lb.this.zone_id
}

output "target_group_arn" {
  description = "Target group ARN."
  value       = aws_lb_target_group.this.arn
}

output "security_group_id" {
  description = "Security group ID attached to the ALB."
  value       = aws_security_group.this.id
}
