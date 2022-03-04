# Build and Deploy a Microsoft .NET Core Web API application to AWS App Runner using CloudFormation


In this blog we will show you how to build a Microsoft.NET Web API application with Amazon Aurora Database using  [AWS App Runner](https://aws.amazon.com/apprunner/), a managed service. AWS App Runner makes it easy for developers to quickly deploy containerized web applications and APIs, at scale and with no prior infrastructure experience required. AWS App Runner helps us start with our source code or a container image.

Container workload management tasks, such as managing deployments, scaling infrastructure, or keeping it updated, can be cumbersome and often slows down customers. Since App Runner is a fully managed service that takes care of building and deploying your application, load balancing traffic, or autoscaling up or down per your application needs, using App Runner is a great alternative for orchestrating application deployments for many common application use cases, like API services, backend web services, or websites.

App Runner automatically builds and deploys the web application and load balances traffic with encryption. App Runner also scales up or down automatically to meet your traffic needs. With App Runner, rather than thinking about servers or scaling, you have more time to focus on your applications. Some possible use cases with AWS App Runner are running Micro services, API services, backend web services, websites, etc.,

Furthermore, now that App Runner provides connectivity with other AWS services and resources, such as databases hosted within Amazon Virtual Private Cloud (VPC), you don’t need to have your AWS services publicly accessible. With this feature, App Runner applications can connect to private endpoints in your VPC, and you can enable a more secure and compliant environment by removing public access to these resources. Check out this [post](https://aws.amazon.com/blogs/aws/new-for-app-runner-vpc-support/) to learn more about how to enable a more secure and compliant environment by removing public access to these resources.

You can connect App Runner services to databases in [Amazon Relational Database Service (RDS)](https://aws.amazon.com/rds/), Redis or Memcached caches in [Amazon ElastiCache](https://aws.amazon.com/elasticache/), or your own applications running in [Amazon Elastic Container Service (Amazon ECS)](https://aws.amazon.com/ecs/), [Amazon Elastic Kubernetes Service (EKS)](https://aws.amazon.com/eks/), [Amazon Elastic Compute Cloud (Amazon EC2)](https://aws.amazon.com/ec2/), or on-premises and connected via [AWS Direct Connect](https://aws.amazon.com/directconnect/).  The [VPC connectors](https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/aws-resource-apprunner-vpcconnector.html) in App Runner work similarly to [VPC networking](https://aws.amazon.com/blogs/compute/announcing-improved-vpc-networking-for-aws-lambda-functions/) in [AWS Lambda](https://aws.amazon.com/lambda/) and are based on [AWS Hyperplane](https://www.youtube.com/watch?v=dfEcd3zqPOA&t=4661s), the internal Amazon network function virtualization system behind AWS services and resources like [Network Load Balancer](https://docs.aws.amazon.com/elasticloadbalancing/latest/network/introduction.html), [NAT Gateway](https://docs.aws.amazon.com/vpc/latest/userguide/vpc-nat-gateway.html), and [AWS PrivateLink](https://aws.amazon.com/privatelink/).

Microsoft workloads have a lot of Web APIs that are native to Microsoft methods for serving front-end applications (like ASP.NET, ASP.NET Razor/MVC, ReactJS or Angular Application). These applications traditionally talk to Microsoft SQL Server database for CRUD operations. [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) (EF) Core is a lightweight, extensible, open source and cross-platform version of the popular Entity Framework data access technology. EF Core can serve as an object-relational mapper (O/RM), which enables .NET developers to work with a database using .NET objects. This helps to eliminate the need for most of the data-access code that typically needs to be written. We use MySql.EntityFramework to connect to Amazon RDS in this blog.

Many of these existing .NET Core applications can be containerized using [Docker](https://www.docker.com/) and AWS services like [Amazon EC2](https://aws.amazon.com/ec2/)/ [Amazon Elastic Container Service (ECS)](https://aws.amazon.com/ecs/)/ [Amazon Elastic Kubernetes Service (Amazon EKS)](https://aws.amazon.com/eks), and can interface with databases like [Amazon Aurora Database](https://aws.amazon.com/rds/aurora/) providing a full blown API application system. In our previous blogs, we showed you how to build a similar Microsoft.NET Web API application with Amazon Aurora Database using [AWS CloudFormation](https://aws.amazon.com/blogs/developer/developing-a-microsoft-net-core-web-api-application-with-aurora-database-using-cloudformation/) and [AWS CDK](https://aws.amazon.com/blogs/developer/developing-a-microsoft-net-core-web-api-application-with-aurora-database-using-aws-cdk/). In this blog the application architecture is complemented by build tools like [AWS CodeCommit](https://aws.amazon.com/codecommit/) [AWS CodeBuild](https://aws.amazon.com/codebuild/) using [AWS CloudFormation](https://aws.amazon.com/cloudformation/). We will create a simple Microsoft .NET Web API for a ToDo Application. See below, the solution architecture diagram of Microsoft .NET Core Web API Application Orchestration . The sample TODO application will communicate with an Amazon Aurora Serverless database for basic CRUD operations. Provided CloudFormation templates will build a Web API deployed in AWS App Runner connecting to a RDS Aurora back-end.



![Alt text](aws-apprunner-net-core-rds.png?raw=true "Title")

### AWS services used in this solution

* [Amazon Aurora Serverless](https://aws.amazon.com/rds/aurora/serverless/)is an on-demand, auto-scaling configuration for [Amazon Aurora](https://aws.amazon.com/rds/aurora/). It automatically starts up, shuts down, and scales capacity up or down based on your application's needs.

* [AWS App Runner](https://aws.amazon.com/apprunner/), a managed service that makes it easy for developers to quickly deploy containerized web applications and APIs is used using Docker template containers and allows you to easily run and scale containerized applications on AWS. The example solution for this blog post includes a sample Dockerfile for a .NET Core 3.1 application. Web API code uses AWS SDK Packages like System Manager, MySql EntityFrameworkCore to interact with the database.

* [Amazon Elastic Container Registry (ECR)](https://aws.amazon.com/ecr/), the AWS provided Docker container registry is used and integrated with App Runner, simplifying the development to production workflow.

* [AWS CodeBuild](https://aws.amazon.com/codebuild/) is used to manage the continuous integration of development code into Amazon ECR as a containerized image.

* [AWS CodeCommit](https://aws.amazon.com/codecommit/) is used as the code repository and integrates with AWS CodeBuild for continuous integration process.



### **Prerequisites**

Docker Containers to deploy the Microsoft .NET Web API Application. Make sure Docker daemon is running if you are trying to run the manual scripts (that builds application image and pushes to Amazon ECR). The following are required to setup your development environment:

1. [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
2. Dotnet CLI: Web API application was built using Microsoft .NET core [3.1](https://dotnet.microsoft.com/en-us/download/dotnet/3.1). Please refer Microsoft Documentation for installation.
3. Docker: Install [Docker](https://docs.docker.com/get-docker/) depending on your hardware specification. Make sure the docker daemon is running
4. Additionally, we use AWS SDK, MySql data packages for the Microsoft .NET project and have added them as nuget packages to the solution. In this example, we use Mysql data to connect to MySql/Aurora database and also AWS SDK Systems Manager to connect to Amazon Systems Manager.

```
<PackageReference Include="AWSSDK.SimpleSystemsManagement" Version="3.3.106" />
<PackageReference Include="Amazon.Extensions.Configuration.SystemsManager" Version="1.2.0" />
<PackageReference Include="MySql.EntityFrameworkCore" Version="3.1.10+m8.0.23" />
```

### Solution Walkthrough

Below section will provide steps to provision the infrastructure (and services) and deploy the application. Upon deploying the solution, two AWS CloudFormation stacks namely "net-core-stack-ecr" and "net-core-stack-infra" will be created. CI/CD is achieved through buildspec.yaml file for subsequent checkins on the AWS CodeCommit repo that will be used by the AWS CodeBuild.

#### Clone the code

* Clone the solution code which contains .net api and CloudFormation templates from this [GitHub](https://github.com/shivaramani/aws-apprunner-netcore-app) location. The source code includes two folders — ‘code’ and ‘config’. The ‘code’ folder has the necessary .NET TODO Web API solution, and the config  folder has the AWS CloudFormation templates, and the script files used for deployment.

    * config/net-core-app-ecr.yaml – This template spins up Amazon ECR.
    * config/net-core-app-infra.yaml – This template creates a new VPC with Amazon RDS, AWS App Runner, AWS CodeBuild, AWS CodeCommit, Amazon Systems Manager.
    * config/buildspec.yaml - This yaml is used by CodeBuild. Steps include building the dotnet application, pushes the image to AWS ECR. Upon pushing a new image to AWS ECR, App Runner will redeploy/re-run the application which will be exposed as an endpoint.
    * config/exec_docker.sh - Used by "exec.sh" script when creating the infrastructure for the first time.

* exec.sh (root folder) - This script is used only during the initial setup to push the dotnet application image to AWS ECR. Provide your AWS Account Number to the script as input. Make sure to setup "aws configure" in the cli to the same account number before executing this script.

* cleanup.sh (root folder) - This script is used to clean up the entire deployment for this solution.

#### **Deployment**

This solution can be provisioned in two ways: fully automated with the provided script(s) (option 1) or can be installed using cli commands (option 2).
**Option 1**
Run the "exec.sh". 

```
 chmod +x exec.sh
 ./exec.sh <YOUR_ACCOUNT_NUMBER>
```

* You will notice two CloudFormation stacks are created (net-core-stack-ecr and net-core-stack-infra). Output of the net-core-stack-infra will provide AWS AppRunner Service Url endpoint. Take a few moments to review the services that are spun up in the AWS CloudFormation console.

**Option 2:** Deploy individual CloudFormation stacks and push image to ECR
a. Deploy Amazon ECR CloudFormation stack (config/net-core-app-ecr.yaml)

```
aws cloudformation create-stack --stack-name net-core-stack-ecr --template-body file://config/net-core-app-ecr.yaml  --capabilities CAPABILITY_NAMED_IAM --parameters  ParameterKey=AppStackName,ParameterValue=net-core-stack-ecr

```

b. Build and Deploy application code to Amazon ECR

```
cd code
dotnet build .

ECR_REPO=net-core-stack-repo
ACCOUNT_NUMBER=<YOUR_ACCOUNT_NUMBER>
REGION=us-east-1
APP_SVC=example/apprunnerdotnetsvc

### build the AWS App Runner docker image, tag and push it to AWS ECR
docker build -t $APP_SVC .
docker tag $APP_SVC $ACCOUNT_NUMBER.dkr.ecr.us-east-1.amazonaws.com/$ECR_REPO:latest
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_NUMBER.dkr.ecr.$REGION.amazonaws.com
docker push $ACCOUNT_NUMBER.dkr.ecr.us-east-1.amazonaws.com/$ECR_REPO:latest

cd ..
```

c. Deploy the other CloudFormation stack for infra/services needed for the solution (config/net-core-app-infra.yaml)

```
aws cloudformation create-stack --stack-name net-core-stack-infra --template-body file://config/net-core-app-infra.yaml  --capabilities CAPABILITY_NAMED_IAM --parameters  ParameterKey=AppStackName,ParameterValue=net-core-stack-infra

```

Application CICD

With the above deployment we have successful setup a CICD solution which will enable you to make further updates to the dotnet application code and manage automatic deployment to AWS App Runner. You can clone the code from AWS CodeCommit into a local folder and make updates as needed.

Below are commands to clone and push updates to AWS CodeCommit:

```
# set source directory where you have this current code downloaded from github
SOURCE_DIR=$pwd

# copy and git push steps to codecommit
mkdir codecommit
cd codecommit
git clone https://git-codecommit.us-east-1.amazonaws.com/v1/repos/net-core-stack-ecr-repo
cd ..
mkdir codecommit/net-core-stack-ecr-repo/code codecommit/net-core-stack-ecr-repo/config
cp -R SOURCE_DIR/code/* codecommit/net-core-stack-ecr-repo/code
cp -R SOURCE_DIR/config/* codecommit/net-core-stack-ecr-repo/config
cd codecommit/net-core-stack-ecr-repo
git add .
git commit -m "test cicd commit"
git push
```

### Solution Validation

Verify by visiting the App Runner console to view the application (net-core-stack-apprunner-svc) that has been deployed. In the application, visit the "Logs" tab to view the details about service creation, image pull from ECR, health check on port 8080 before routing traffic to application. A sample Event log will be as below.

```
02-02-2022 04:00:28 PM [AppRunner] Service status is set to RUNNING.
02-02-2022 04:00:28 PM [AppRunner] Service creation completed successfully.
02-02-2022 04:00:27 PM [AppRunner] Successfully routed incoming traffic to application.
02-02-2022 03:59:56 PM [AppRunner] Health check is successful. Routing traffic to application.
02-02-2022 03:58:59 PM [AppRunner] Performing health check on port '8080'.
02-02-2022 03:58:51 PM [AppRunner] Provisioning instances and deploying image.
02-02-2022 03:58:39 PM [AppRunner] Successfully pulled image from ECR.
02-02-2022 03:56:54 PM [AppRunner] Successfully created pipeline for automatic deployments.
02-02-2022 03:56:35 PM [AppRunner] Service status is set to OPERATION_IN_PROGRESS.
02-02-2022 03:56:35 PM [AppRunner] Service creation started.
```

Go to AWS CloudFormation console and in the Infra stack output tab get the "AppRunnerServiceUrl" to test the health check api on a browser. A sample url will be like - http://gcdpsgaztx.us-east-1.awsapprunner.com/api/values
To test the application endpoint, you can use tools like Postman, ARC Rest Client or Browser extensions like RestMan. Select "content-type" as "application/json", POST as raw data/json - sample input below - https://your_app_runner_url/api/todo

```
{
   "Task": "new TODO Application",
   "Status": "Done"
}
```

Use the same url and fire a GET call to see the previously posted todo item as response.

```
https://<YOUR_APP_RUNNER_URL>/api/todo/Done
```

### Clean Up

Option 1: You can run the provided "cleanup.sh" to clean up the entire solution

```
chmod +x cleanup.sh
.\cleanup.sh
```

Option 2: If you like to individually execute the following commands.

```
aws ecr batch-delete-image --repository-name net-core-stack-ecr-repo --image-ids imageTag=latest

aws ecr batch-delete-image --repository-name net-core-stack-ecr-repo --image-ids imageTag=untagged`Delete the entire infrastructure
```

```
aws cloudformation delete-stack --stack-name net-core-stack-ecr

aws cloudformation delete-stack --stack-name net-core-stack-infra
```

### Troubleshooting

* **AWS CloudFormation Stacks Failure:** If you notice any of the CloudFormation stack failures below steps would help to triage.
    * ecr-stack: Make sure Docker daemon is running; Check the CloudFormation "Outputs" of the ecr-stack to verify the ECR arn. Look at the Events tab to see if any failures have occurred.

    * infra stack: Review the AWS service limits. Ex: 5 VPCs per region; Make sure ECR stack is created with docker image (application code as docker container images in there). Look at the Events tab to see if any failures have occurred.

* **RDSCluster Connectivity**

    * Use AWS RDS console to verify the new RDS cluster. Connectivity to RDS Endpoints/credentials will be in AWS Systems Manager.
    * Table/Model entity will not be created unless TODO Url POST is triggered.
    * Note: Inactivity in Aurora Serverless – RDS Table could put the RDS in suspended state to reduce the cost. You might receive a communication error after no activity while trying to invoke the database DDL/DML statements. If the application provided API times out (especially after the initial setup, you may have to invoke the POST/GET call invoke on the /api/todo endpoint). You might notice this by connecting to the SQL in Query Editor with below output.Retrying the select queries will warm up the RDS database for subsequent connection to be served.
        * `Communications link failure The last packet sent successfully to the server was 0 milliseconds ago. The driver has not received any packets from the server.`
* **AWS AppRunner**

    * Verify App Runner application "Event Logs" and "Deployment logs".
    * By default, the application is coded to run in port 8080. Make sure they are seen in application log.
    * Check the health check url to see the test endpoint
    * Check the TODO Url to post the data from tools like Postman or RestMan (Google Chrome/MS Edge plugin). Check if the data is available in RDS database.

### References

* [AWS App Runner Documentation](https://docs.aws.amazon.com/apprunner/)
* [New for App Runner – VPC Support | AWS News Blog (amazon.com)](https://aws.amazon.com/blogs/aws/new-for-app-runner-vpc-support/)
* [Networking with App Runner - AWS App Runner (amazon.com)](https://docs.aws.amazon.com/apprunner/latest/dg/network.html)
* [Developing a Microsoft .NET Core Web API application with Aurora Database using CloudFormation](https://aws.amazon.com/blogs/developer/developing-a-microsoft-net-core-web-api-application-with-aurora-database-using-cloudformation/)
* [Build and deploy a Spring Boot application to AWS App Runner with a CI/CD pipeline using Terraform](https://aws.amazon.com/blogs/containers/build-and-deploy-a-spring-boot-application-to-aws-app-runner-with-a-ci-cd-pipeline-using-terraform/)

