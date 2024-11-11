using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;

public class SecurityGroupsModule
{
    public SecurityGroup BastionHostSg { get; private set; }
    public SecurityGroup PrivateEc2Sg { get; private set; }
    public SecurityGroup LbSg { get; private set; }
    public SecurityGroup RdsMainSg { get; private set; }
    public SecurityGroup RdsTestSg { get; private set; }

    public SecurityGroupsModule(Vpc vpc, string vpcCidrBlock)
    {

        BastionHostSg = new SecurityGroup("bastionHostSecurityGroup", new()
        {
            VpcId = vpc.Id,
            Ingress = new[]
            {
            new SecurityGroupIngressArgs
            {
                FromPort = 22,
                ToPort = 22,
                Protocol = "tcp",
                CidrBlocks = new[]
                {
                    "0.0.0.0/0",
                },
            },
            new SecurityGroupIngressArgs
            {
                FromPort = 8086,
                ToPort = 8086,
                Protocol = "tcp",
                CidrBlocks = new[]
                {
                    "0.0.0.0/0",
                },
            },
        },
            Egress = new[]
                {
            new SecurityGroupEgressArgs
            {
                FromPort = 0,
                ToPort = 0,
                Protocol = "-1",
                CidrBlocks = new[] { "0.0.0.0/0" },
            }
            },
        });


        LbSg = new SecurityGroup("lbSecurityGroup", new()
        {
            VpcId = vpc.Id,
            Ingress = new[]
                {
            new SecurityGroupIngressArgs
            {
                FromPort = 80,
                ToPort = 80,
                Protocol = "tcp",
                CidrBlocks = new[] { "0.0.0.0/0" },
            },
        },
            Egress = new[]
                {
            new SecurityGroupEgressArgs
            {
                FromPort = 0,
                ToPort = 0,
                Protocol = "-1",
                CidrBlocks = new[] { "0.0.0.0/0" },
            },
        }
        });


        PrivateEc2Sg = new SecurityGroup("privateEc2Sg", new SecurityGroupArgs
        {
            VpcId = vpc.Id,
            Ingress =
        {
            new SecurityGroupIngressArgs
            {
                FromPort = 22,
                ToPort = 22,
                Protocol = "tcp",
                CidrBlocks = new[] { "0.0.0.0/0" }
            },
            new SecurityGroupIngressArgs
            {
                FromPort = 8000,
                ToPort = 8000,
                Protocol = "tcp",
                CidrBlocks = new[] { "0.0.0.0/0" }
            }
        },
            Egress =
        {
            new SecurityGroupEgressArgs
            {
                FromPort = 0,
                ToPort = 0,
                Protocol = "-1",
                CidrBlocks = new[] { "0.0.0.0/0" }
            },
        },
        });





        RdsMainSg = new SecurityGroup("rdsSecurityGroupMain", new SecurityGroupArgs
        {
            VpcId = vpc.Id,
            Egress =
            {
                new SecurityGroupEgressArgs
                {
                    Protocol = "-1",
                    FromPort = 0,
                    ToPort = 0,
                    CidrBlocks = "0.0.0.0/0"
                }
            },
            Ingress =
            {
                new SecurityGroupIngressArgs
                {
                    Protocol = "tcp",
                    FromPort = 3306,
                    ToPort = 3306,
                    CidrBlocks = vpcCidrBlock
                }
            }
        });


        RdsTestSg = new SecurityGroup("rdsSecurityGroupTest", new SecurityGroupArgs
        {
            VpcId = vpc.Id,
            Egress =
            {
                new SecurityGroupEgressArgs
                {
                    Protocol = "-1",
                    FromPort = 0,
                    ToPort = 0,
                    CidrBlocks = "0.0.0.0/0"
                }
            },
            Ingress =
            {
                new SecurityGroupIngressArgs
                {
                    Protocol = "-1",
                    FromPort = 0,
                    ToPort = 0,
                    CidrBlocks = "0.0.0.0/0"
                }
            }
        });
    }
}
