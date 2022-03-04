## Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved
##
### SPDX-License-Identifier: MIT-0

#!/bin/bash
#### Pass region and account number

cd ..
cd code

dotnet build .

ECR_REPO=$1
ACCOUNT_NUMBER=$2
REGION=us-east-1
APP_SVC=example/apprunnerdotnetsvc


### build the AWS App Runner docker image, tag and push it to AWS ECR
docker build -t $APP_SVC .
docker tag $APP_SVC $ACCOUNT_NUMBER.dkr.ecr.us-east-1.amazonaws.com/$ECR_REPO:latest
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_NUMBER.dkr.ecr.$REGION.amazonaws.com
docker push $ACCOUNT_NUMBER.dkr.ecr.us-east-1.amazonaws.com/$ECR_REPO:latest

#### windows user can stop closing the window to see the output
#cmd /k