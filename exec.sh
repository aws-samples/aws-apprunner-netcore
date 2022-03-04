## Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved
##
### SPDX-License-Identifier: MIT-0

#!/bin/bash
#### Pass region and account number

if [ -z "$1" ]
then
      echo "AccountID is empty"
      
      #### windows users can stop closing the window to see the output
      #cmd /k
      exit 1
fi

echo 'Creating Stacks....'

# Note this stack name is prefixed in infra yaml for ECRRepo Parameter
STACK_NAME=net-core-stack
aws cloudformation create-stack --stack-name $STACK_NAME-ecr --template-body file://config/net-core-app-ecr.yaml  --capabilities CAPABILITY_NAMED_IAM --parameters  ParameterKey=AppStackName,ParameterValue=$STACK_NAME-ecr
cd config
./exec_docker.sh $STACK_NAME-ecr-repo $1
cd ..
aws cloudformation create-stack --stack-name $STACK_NAME-infra --template-body file://config/net-core-app-infra.yaml  --capabilities CAPABILITY_NAMED_IAM --parameters  ParameterKey=AppStackName,ParameterValue=$STACK_NAME-infra

echo 'Steps completed successfully!'

#### windows users can stop closing the window to see the output
#cmd /k