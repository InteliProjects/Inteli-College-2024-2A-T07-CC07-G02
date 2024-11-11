using Aws = Pulumi.Aws;
using Pulumi.Aws.S3;

public class BucketModule
{
    public Bucket Bucket { get; private set; }
    public BucketPolicy Policy { get; private set; }

    public BucketModule(string name)
    {

        Bucket = new Bucket($"{name}-frontend-bucket", new()
        {
            Website = new Aws.S3.Inputs.BucketWebsiteArgs
            {
                IndexDocument = "index.html",
                ErrorDocument = "error.html",
            }
        });

        var bucketPublicAccessBlock = new BucketPublicAccessBlock("bucketPublicAccessBlock", new BucketPublicAccessBlockArgs
        {
            Bucket = Bucket.Id,
            BlockPublicAcls = false,         // Allow public ACLs
            IgnorePublicAcls = false,        // Don't ignore public ACLs
            BlockPublicPolicy = false,       // Allow bucket policies to grant public access
            RestrictPublicBuckets = false    // Don't restrict public access to the bucket
        });

        Policy = new BucketPolicy("bucketPolicy", new()
        {
            Bucket = Bucket.BucketName,

            Policy = Bucket.BucketName.Apply(bucketName =>
                @$"{{
                    ""Version"": ""2012-10-17"",
                    ""Statement"": [
                        {{
                            ""Effect"": ""Allow"",
                            ""Principal"": ""*"",
                            ""Action"": ""s3:GetObject"",
                            ""Resource"": ""arn:aws:s3:::{bucketName}/*""
                        }}
                    ]
                }}")
        });

    }
}
