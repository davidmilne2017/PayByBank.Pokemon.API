using Xunit;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using PayByBank.Pokemon.Common.Constants;
using Moq.Protected;
using System.Threading;
using PayByBank.Pokemon.Common.Domain.Translation;
using System.Text.Json;
using System;
using PayByBank.Pokemon.Infrastructure.Repositories;
using System.Web;
using Microsoft.Extensions.Logging;

namespace PayByBank.Pokemon.Infrastructure.Tests.Repositories
{
    public class TranslationHttpRepositoryTests
    {

        const string resource = "https://test.com/";

        [Theory]
        [InlineData("original", "translation", "translation", 1)]
        [InlineData("original", "translation", "original", 0)]
        public async Task TranslateTextAsync_CorrectResponse_ReturnsTranslatedText(string text, string translatedText, string expReturn, int total)
        {
            //Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock.Setup(x => x.Value).Returns(resource);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection(It.Is<string>(k => k == ConstantValues.TranslationApi))).Returns(configurationSectionMock.Object);
            
            var translationType = TranslationType.YODA;
            var translationResponse = CreateTranslationReponse(text, translatedText, total);
            var translatedResponseJson = JsonSerializer.Serialize(translationResponse, translationResponse.GetType(), Options);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(translatedResponseJson),
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(resource),
            };

            var loggerMock = new Mock<ILogger<TranslationHttpRepository>>();
            var sut = new TranslationHttpRepository(httpClientFactoryMock.Object, configurationMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            //Act
            var result = await sut.TranslateText(text, translationType, token);

            //Assert
            result.Should().NotBeNull();
            result.Should().Be(expReturn);

            var expectedUri = generateUri(resource,translationType,text);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task TranslateTextAsync_IncompleteResponseNoSuccess_ReturnsOriginalText()
        {
            //Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock.Setup(x => x.Value).Returns(resource);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection(It.Is<string>(k => k == ConstantValues.TranslationApi))).Returns(configurationSectionMock.Object);

            var translationType = TranslationType.YODA;
            var text = "Not translated text";
            var translatedText = "Translated text";
            var translationResponse = CreateTranslationReponse(text, translatedText, 1);
            translationResponse.Success = null;
            var translatedResponseJson = JsonSerializer.Serialize(translationResponse, translationResponse.GetType(), Options);            

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(translatedResponseJson),
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(resource),
            };

            var loggerMock = new Mock<ILogger<TranslationHttpRepository>>();
            var sut = new TranslationHttpRepository(httpClientFactoryMock.Object, configurationMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            //Act
            var result = await sut.TranslateText(text, translationType, token);

            //Assert
            result.Should().NotBeNull();
            result.Should().Be(text);

            var expectedUri = generateUri(resource, translationType, text);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task TranslateTextAsync_IncompleteResponseNoContents_ReturnsOriginalText()
        {
            //Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock.Setup(x => x.Value).Returns(resource);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection(It.Is<string>(k => k == ConstantValues.TranslationApi))).Returns(configurationSectionMock.Object);

            var translationType = TranslationType.YODA;
            var text = "Not translated text";
            var translatedText = "Translated text";
            var translationResponse = CreateTranslationReponse(text, translatedText, 1);
            translationResponse.Contents = null;
            var translatedResponseJson = JsonSerializer.Serialize(translationResponse, translationResponse.GetType(), Options);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(translatedResponseJson),
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(resource),
            };

            var loggerMock = new Mock<ILogger<TranslationHttpRepository>>();
            var sut = new TranslationHttpRepository(httpClientFactoryMock.Object, configurationMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            //Act
            var result = await sut.TranslateText(text, translationType, token);

            //Assert
            result.Should().NotBeNull();
            result.Should().Be(text);

            var expectedUri = generateUri(resource, translationType, text);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task TranslateTextAsync_NullResponse_ReturnsOriginalText()
        {
            //Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock.Setup(x => x.Value).Returns(resource);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection(It.Is<string>(k => k == ConstantValues.TranslationApi))).Returns(configurationSectionMock.Object);

            var text = "Not translated text";
            var translationType = TranslationType.YODA;

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new InvalidOperationException())
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(resource),
            };

            var loggerMock = new Mock<ILogger<TranslationHttpRepository>>();
            var sut = new TranslationHttpRepository(httpClientFactoryMock.Object, configurationMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            //Act
            var result = await sut.TranslateText(text, translationType, token);

            //Assert
            result.Should().Be(text);
        }

        private TranslationResponse CreateTranslationReponse(string text, string translatedText, int total)
        {
            return new TranslationResponse()
            {
                Success = new TranslationResponseSuccess()
                {
                    Total = total
                },
                Contents = new TranslationResponseContents()
                {
                    Text = text,
                    Translated = translatedText
                 }
            };
        }

        private Uri generateUri(string resource, TranslationType translationType, string text)
        {
            var builder = new UriBuilder($"{resource}{translationType.ToString().ToLower()}");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["text"] = text;
            builder.Query = query.ToString();
            return builder.Uri;
        }

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}
