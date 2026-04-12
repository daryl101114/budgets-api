output "elastic_ip" {
  description = "Static public IP of the EC2 instance"
  value       = aws_eip.main.public_ip
}

output "ec2_instance_id" {
  description = "EC2 instance ID"
  value       = aws_instance.main.id
}

output "ssh_command" {
  description = "SSH command to connect to the server"
  value       = "ssh -i ~/.ssh/budget-api-key.pem ubuntu@${aws_eip.main.public_ip}"
}
