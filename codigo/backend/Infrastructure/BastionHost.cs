using Pulumi.Aws.Ec2;

public class BastionHostModule
{
    public Instance BastionHost { get; private set; }

    public BastionHostModule(string bastionHostKey, SecurityGroup bastionHostSg, Subnet publicSubnet, string amiId)
    {
        BastionHost = new Instance("bastionHost", new()
        {
            InstanceType = "t3.micro",
            SubnetId = publicSubnet.Id,
            VpcSecurityGroupIds = new[] { bastionHostSg.Id },
            Ami = amiId,
            KeyName = bastionHostKey,
            Tags = { { "Name", "bastion-host" } },
        });
    }
}
