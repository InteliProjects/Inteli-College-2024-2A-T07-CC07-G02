using Pulumi.Aws.Rds;
using Aws = Pulumi.Aws;
using DotNetEnv;


public class RdsModule
{
    // RDS Instances
    public Instance MainDb { get; private set; }
    public Instance TestDb { get; private set; }

    // Subnet Groups
    public SubnetGroup MainDbSubnetGroup { get; private set; }
    public SubnetGroup TestDbSubnetGroup { get; private set; }


    public RdsModule(
            Aws.Ec2.Subnet publicSubnet1,
            Aws.Ec2.Subnet publicSubnet2,
            Aws.Ec2.Subnet privateSubnet1,
            Aws.Ec2.Subnet privateSubnet2,
            Aws.Ec2.SecurityGroup rdsMainSg,
            Aws.Ec2.SecurityGroup rdsTestSg,
            string dbName,
            string dbUsername,
            string dbPassword)
    {
        MainDbSubnetGroup = new SubnetGroup("main-db-subnet-group", new SubnetGroupArgs
        {
            Name = "main-db-subnet-group",
            Description = "DB Subnets",
            SubnetIds = new[] { privateSubnet1.Id, privateSubnet2.Id }
        });

        TestDbSubnetGroup = new SubnetGroup("test-db-subnet-group", new SubnetGroupArgs
        {
            Name = "test-db-subnet-group",
            Description = "DB Subnets",
            SubnetIds = new[] { publicSubnet1.Id, publicSubnet2.Id }
        });

        MainDb = new Instance("main-db", new InstanceArgs
        {
            AllocatedStorage = 20,
            Engine = "mysql",
            InstanceClass = "db.t3.micro",
            DbName = dbName,
            Username = dbUsername,
            Password = dbPassword,
            DbSubnetGroupName = MainDbSubnetGroup.Name,
            VpcSecurityGroupIds = { rdsMainSg.Id },
            SkipFinalSnapshot = true
        });

        TestDb = new Instance("test-db", new InstanceArgs
        {
            AllocatedStorage = 20,
            Engine = "mysql",
            InstanceClass = "db.t3.micro",
            DbName = dbName,
            Username = dbUsername,
            Password = dbPassword,
            DbSubnetGroupName = TestDbSubnetGroup.Name,
            VpcSecurityGroupIds = { rdsTestSg.Id },
            PubliclyAccessible = true,
            SkipFinalSnapshot = true
        });
    }
}
