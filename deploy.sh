#!/bin/bash

# Variables - customize these
PEM_KEY=~/.ssh/N-Virginia-Daryl-Win-Desktop.pem
USER=ubuntu
HOST=3.88.109.37
REMOTE_DIR=/var/www/budgifyapi
SERVICE_NAME=budgifyapi.service

# Publish .NET app to 'publish' folder
echo "Publishing .NET app..."
dotnet publish -c Release -o ./publish

# Sync published files to remote server
echo "Syncing files to server..."
scp -i ~/.ssh/N-Virginia-Daryl-Win-Desktop.pem -r ./publish/ ubuntu@3.88.109.37:/var/www/budgifyapi


# SSH into server and restart the service
echo "Restarting systemd service..."
ssh -i $PEM_KEY $USER@$HOST << EOF
  sudo systemctl restart $SERVICE_NAME
  sudo systemctl status $SERVICE_NAME --no-pager
EOF

echo "Deployment complete!"
