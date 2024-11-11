using Pulumi;
using Pulumi.Aws.Ec2;

public class VpcModule
{
    public Vpc Vpc { get; private set; }

    // Subnetworks
    public Subnet PublicSubnet1 { get; private set; }
    public Subnet PublicSubnet2 { get; private set; }
    public Subnet PrivateSubnet1 { get; private set; }
    public Subnet PrivateSubnet2 { get; private set; }

    // Gateways
    public InternetGateway InternetGateway { get; private set; }
    public NatGateway NatGateway1 { get; private set; }
    public NatGateway NatGateway2 { get; private set; }

    // Route Tables
    public RouteTable PublicRouteTable { get; private set; }
    public RouteTable PrivateRouteTable1 { get; private set; }
    public RouteTable PrivateRouteTable2 { get; private set; }

    // Route Tables Associations
    public RouteTableAssociation PublicSubnet1Association { get; private set; }
    public RouteTableAssociation PublicSubnet2Association { get; private set; }
    public RouteTableAssociation PrivateSubnet1Association { get; private set; }
    public RouteTableAssociation PrivateSubnet2Association { get; private set; }

    public Output<string> VpcId => Vpc.Id;

    public VpcModule(string name, string cidrBlock)
    {
        var availabilityZones = Pulumi.Aws.GetAvailabilityZones.InvokeAsync().Result;

        Vpc = new Vpc(name, new VpcArgs
        {
            CidrBlock = cidrBlock,
            EnableDnsSupport = true,
            EnableDnsHostnames = true,
        });

        PublicSubnet1 = new Subnet("publicSubnet1", new()
        {
            VpcId = Vpc.Id,
            CidrBlock = "10.0.0.0/24",
            MapPublicIpOnLaunch = true,
            AvailabilityZone = availabilityZones.Names[0],
            Tags =
            {
                { "Name", "PublicSubnet1" },
            }
        });

        PublicSubnet2 = new Subnet("publicSubnet2", new()
        {
            VpcId = Vpc.Id,
            CidrBlock = "10.0.16.0/24",
            MapPublicIpOnLaunch = true,
            AvailabilityZone = availabilityZones.Names[1],
            Tags =
            {
                { "Name", "PublicSubnet2" },
            }
        });

        PrivateSubnet1 = new Subnet("privateSubnet1", new()
        {
            VpcId = Vpc.Id,
            CidrBlock = "10.0.128.0/24",
            AvailabilityZone = availabilityZones.Names[0],
            Tags =
            {
                { "Name", "PrivateSubnet1" },
            }
        });

        PrivateSubnet2 = new Subnet("privateSubnet2", new()
        {
            VpcId = Vpc.Id,
            CidrBlock = "10.0.144.0/24",
            AvailabilityZone = availabilityZones.Names[1],
            Tags =
            {
                { "Name", "PrivateSubnet2" },
            }
        });

        InternetGateway = new InternetGateway($"{name}-igw", new()
        {
            VpcId = Vpc.Id
        });

        NatGateway1 = new NatGateway($"{name}-nat-gateway1", new()
        {
            SubnetId = PublicSubnet1.Id,
            ConnectivityType = "public",
            AllocationId = new Pulumi.Aws.Ec2.Eip("natGatewayEip1").Id
        });


        NatGateway2 = new NatGateway($"{name}-nat-gateway2", new()
        {
            SubnetId = PublicSubnet2.Id,
            ConnectivityType = "public",
            AllocationId = new Pulumi.Aws.Ec2.Eip("natGatewayEip2").Id

        });

        PublicRouteTable = new RouteTable($"{name}-public-route-table", new()
        {
            VpcId = Vpc.Id,
            Routes = new[]{
            new Pulumi.Aws.Ec2.Inputs.RouteTableRouteArgs{
                CidrBlock = "0.0.0.0/0",
                GatewayId = InternetGateway.Id,
            }
            }
        });




        PrivateRouteTable1 = new RouteTable($"{name}-private-route-table1", new()
        {
            VpcId = Vpc.Id,
            Routes = new[]{
            new Pulumi.Aws.Ec2.Inputs.RouteTableRouteArgs{
                CidrBlock = "0.0.0.0/0",
                NatGatewayId = NatGateway1.Id,
            }
            }

        });

        PrivateRouteTable2 = new RouteTable($"{name}-private-route-table2", new()
        {
            VpcId = Vpc.Id,
            Routes = new[]{
            new Pulumi.Aws.Ec2.Inputs.RouteTableRouteArgs{
                CidrBlock = "0.0.0.0/0",
                NatGatewayId = NatGateway2.Id,
            }
            }

        });

        PublicSubnet1Association = new RouteTableAssociation($"{name}-public-subnet1-association", new RouteTableAssociationArgs
        {
            SubnetId = PublicSubnet1.Id,
            RouteTableId = PublicRouteTable.Id
        });

        PublicSubnet2Association = new RouteTableAssociation($"{name}-public-subnet2-association", new RouteTableAssociationArgs
        {
            SubnetId = PublicSubnet2.Id,
            RouteTableId = PublicRouteTable.Id
        });

        PrivateSubnet1Association = new RouteTableAssociation($"{name}-private-subnet1-association", new RouteTableAssociationArgs
        {
            SubnetId = PrivateSubnet1.Id,
            RouteTableId = PrivateRouteTable1.Id
        });

        PrivateSubnet2Association = new RouteTableAssociation($"{name}-private-subnet2-association", new RouteTableAssociationArgs
        {
            SubnetId = PrivateSubnet2.Id,
            RouteTableId = PrivateRouteTable2.Id
        });
    }
}
