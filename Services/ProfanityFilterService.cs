namespace ThomasianMemoir.Services
{
    using Google.Apis.Auth.OAuth2;
    using Google.Cloud.Language.V1;
    using Google.Protobuf;
    using Grpc.Core;
    using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

    public class ProfanityFilterService
    {
        private readonly LanguageServiceClient languageServiceClient;

        public ProfanityFilterService()
        {
            GoogleCredential credential = GoogleCredential.FromFile("ServiceAccountKeys/thomasian-memoir-18e7e8bfbad0.json");

            var builder = new LanguageServiceClientBuilder
            {
                CredentialsPath = "ServiceAccountKeys/thomasian-memoir-18e7e8bfbad0.json"
            };

            languageServiceClient = builder.Build();
        }

        public bool ContainsBadWords(string text)
        {
            Document document = new Document
            {
                Content = text,
                Type = Document.Types.Type.PlainText,
            };

            try
            {
                AnalyzeSentimentResponse response = languageServiceClient.AnalyzeSentiment(document);
                return response.DocumentSentiment.Score < -0.5;
            }
            catch (RpcException ex)
            {
                if (ex.Status.StatusCode == StatusCode.InvalidArgument && ex.Status.Detail.Contains("language"))
                {
                    Console.WriteLine("Language not supported for sentiment analysis.");
                    return false;
                }
                throw;
            }
        }
    }
}