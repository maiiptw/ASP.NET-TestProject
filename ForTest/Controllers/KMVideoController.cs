using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KMVideoController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public KMVideoController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult getKmVideo([FromHeader(Name = "x-CustomHeader2")] string uuid)
        {
            string filename = uuid+".mp4";
            // Create a client
            AmazonS3Client client = new AmazonS3Client(Configuration["KM_S3:AccessKeyID"], Configuration["KM_S3:SecretAccesskey"], Amazon.RegionEndpoint.APSoutheast1);

            // Create a GetObject request
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = Configuration["KM_S3:BucketName"],
                Key = uuid
            };
            // Issue request and remember to dispose of the response
            Task<GetObjectResponse> response = client.GetObjectAsync(request);
            response.Wait();

            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.Headers.Add("Content-Length", response.Result.ContentLength.ToString());

            return File(response.Result.ResponseStream, contentType: "video/mp4", fileDownloadName: filename, enableRangeProcessing: true);
        }
    }
}
