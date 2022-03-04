## Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved
##
### SPDX-License-Identifier: MIT-0

#!/bin/bash
#### Pass region and account number

STACK_NAME=net-core-stack

aws ecr batch-delete-image --repository-name $STACK_NAME-ecr-repo --image-ids imageTag=latest

aws ecr batch-delete-image --repository-name $STACK_NAME-ecr-repo --image-ids imageTag=untagged

aws cloudformation delete-stack --stack-name $STACK_NAME-ecr

aws cloudformation delete-stack --stack-name $STACK_NAME-infra

#### windows users can stop closing the window to see the output
#cmd /k