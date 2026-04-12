variable "aws_region" {
  description = "AWS region to deploy resources in"
  type        = string
  default     = "us-east-1"
}

variable "instance_type" {
  description = "EC2 instance type"
  type        = string
  default     = "t3.small"
}

variable "my_ip" {
  description = "Your public IP address for SSH access (format: x.x.x.x/32)"
  type        = string
}

variable "allowed_ips" {
  description = "List of IP addresses allowed to access the API on ports 80/443 (format: x.x.x.x/32)"
  type        = list(string)
}

variable "key_name" {
  description = "Name of the EC2 key pair to use for SSH access"
  type        = string
}

variable "project_name" {
  description = "Project name used to tag and name AWS resources"
  type        = string
  default     = "budgify"
}
