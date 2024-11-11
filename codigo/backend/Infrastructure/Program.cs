using System.IO;
using System.Collections.Generic;
using Pulumi;
using Aws = Pulumi.Aws;
using DotNetEnv;

Env.Load("../.env");

return await Deployment.RunAsync(() =>
{
    var config = new Config();
    var instanceType = config.Get("instanceType") ?? "t3.micro";
    var vpcNetworkCidr = config.Get("vpcNetworkCidr") ?? "10.0.0.0/16";
    var bastionHostPemKey = File.ReadAllText("./bastion-host.pem");
    var backendPemKey = File.ReadAllText("./backendKey.pem");
    var dbName = Env.GetString("DB_NAME");
    var dbUsername = Env.GetString("DB_USERNAME");
    var dbPassword = Env.GetString("DB_PASSWORD");


    var amiId = "ami-0e86e20dae9224db8"; // Example for Ubuntu 20.04 LTS (HVM), SSD Volume Type in us-east-1


    // Path to the user data file
    var userDataScriptPath = "./boot.sh";

    // Read the user data script from the file
    var userData = File.ReadAllText(userDataScriptPath);


    var vpcModule = new VpcModule("nimbus", "10.0.0.0/16");
    var securityGroups = new SecurityGroupsModule(vpcModule.Vpc, "10.0.0.0/16");

    // Create a bastion host (bastion host).
    var bastionHost = new BastionHostModule(
            "bastion-host",
            securityGroups.BastionHostSg,
            vpcModule.PublicSubnet1,
            amiId);

    var autoScaleModule = new AutoScaleModule(
            vpcModule,
            instanceType,
            securityGroups.LbSg,
            securityGroups.PrivateEc2Sg,
            "backendKey",
            userData,
            amiId);


    var rds = new RdsModule(
        vpcModule.PublicSubnet1,
        vpcModule.PublicSubnet2,
        vpcModule.PrivateSubnet1,
        vpcModule.PrivateSubnet2,
        securityGroups.RdsMainSg,
        securityGroups.RdsTestSg,
        dbName,
        dbUsername,
        dbPassword);

    var bucketModule = new BucketModule("nimbus");

    // Export important information.
    return new Dictionary<string, object?>
    {
        ["bastionHostIp"] = bastionHost.BastionHost.PublicIp,
        ["loadBalancerDns"] = autoScaleModule.Lb.DnsName,
        ["s3BucketName"] = bucketModule.Bucket.BucketName,
        ["s3BucketEndpoint"] = bucketModule.Bucket.WebsiteEndpoint,
        ["main-db-endpoint"] = rds.MainDb.Endpoint,
        ["main-db-port"] = rds.MainDb.Port,
        ["test-db-endpoint"] = rds.TestDb.Endpoint,
        ["test-db-port"] = rds.TestDb.Port,
    };
});
