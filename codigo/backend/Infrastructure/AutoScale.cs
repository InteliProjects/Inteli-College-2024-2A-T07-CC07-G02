using Pulumi.Aws.Ec2;
using Pulumi.Aws.LB;
using Pulumi.Aws.AutoScaling;
using Aws = Pulumi.Aws;
using System;
using System.Text;

public class AutoScaleModule
{
    public LaunchTemplate LaunchTemplate { get; private set; }
    public Group AutoScaleGroup { get; set; }
    public LoadBalancer Lb { get; private set; }
    public TargetGroup TargetGroup { get; private set; }
    public Listener Listener { get; private set; }

    public AutoScaleModule(VpcModule vpcModule, string instanceType, SecurityGroup lbSg, SecurityGroup privateEc2Sg, string backendKey, string userData, string amiId)
    {

        Lb = new LoadBalancer("lb", new LoadBalancerArgs
        {
            SecurityGroups = new[] { lbSg.Id },
            Subnets = new[] { vpcModule.PublicSubnet1.Id, vpcModule.PublicSubnet2.Id },
            LoadBalancerType = "application",
        });

        TargetGroup = new TargetGroup("targetGroup", new TargetGroupArgs
        {
            Port = 8000,
            Protocol = "HTTP",
            VpcId = vpcModule.Vpc.Id,
            HealthCheck = new Aws.LB.Inputs.TargetGroupHealthCheckArgs
            {
                Path = "/health",               // Health check route
                Protocol = "HTTP",              // Protocol for the health check
                Port = "8000",                  // Health check port
                Interval = 10,                  // Interval between health checks (seconds)
                Timeout = 5,                    // Timeout before considering it failed
                HealthyThreshold = 2,           // Number of healthy checks required to mark as healthy
                UnhealthyThreshold = 2,         // Number of unhealthy checks before marked as unhealthy
            }
        });

        Listener = new Listener("lb-listener", new ListenerArgs
        {
            LoadBalancerArn = Lb.Arn,
            Port = 80,
            DefaultActions = new[]
            {
                new Aws.LB.Inputs.ListenerDefaultActionArgs
                {
                    Type = "forward",
                    TargetGroupArn = TargetGroup.Arn
                }
            },
            Protocol = "HTTP",
        });


        LaunchTemplate = new Aws.Ec2.LaunchTemplate("launchTemplate", new()
        {
            InstanceType = instanceType,
            ImageId = amiId,
            VpcSecurityGroupIds = new[] { privateEc2Sg.Id },
            KeyName = backendKey,
            UserData = Convert.ToBase64String(Encoding.UTF8.GetBytes(userData))
        });

        AutoScaleGroup = new Group("autoScalingGroup", new GroupArgs
        {
            LaunchTemplate = new Aws.AutoScaling.Inputs.GroupLaunchTemplateArgs
            {
                Id = LaunchTemplate.Id,
                Version = "$Latest"
            },
            MinSize = 1,
            MaxSize = 3,
            DesiredCapacity = 1,
            VpcZoneIdentifiers = new[]
                {
            vpcModule.PrivateSubnet1.Id,
            vpcModule.PrivateSubnet2.Id,
        },
            TargetGroupArns = { TargetGroup.Arn },
            Tags = new[]
            {
                new Pulumi.Aws.AutoScaling.Inputs.GroupTagArgs
                {
                    Key = "Name",
                    Value = "backend",  // Name template for the instances
                    PropagateAtLaunch = true // Ensure the tag is applied when instances are created
                }
            }
        });
    }
}
