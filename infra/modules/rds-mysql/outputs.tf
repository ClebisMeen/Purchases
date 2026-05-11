output "address" {
  description = "RDS endpoint address."
  value       = aws_db_instance.this.address
}

output "endpoint" {
  description = "RDS endpoint including port."
  value       = aws_db_instance.this.endpoint
}

output "port" {
  description = "RDS port."
  value       = aws_db_instance.this.port
}

output "db_name" {
  description = "Database name."
  value       = aws_db_instance.this.db_name
}

output "username" {
  description = "Master username."
  value       = aws_db_instance.this.username
}

output "password" {
  description = "Master password."
  value       = random_password.master.result
  sensitive   = true
}

output "security_group_id" {
  description = "Security group ID for the database."
  value       = aws_security_group.this.id
}

output "db_instance_arn" {
  description = "RDS instance ARN."
  value       = aws_db_instance.this.arn
}
