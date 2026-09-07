using System;
using System.ComponentModel.DataAnnotations;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Test.Options;

public class GraphOptionsTest
{
    [Fact]
    public void GraphOptions_ValidProperties_ShouldPassValidation()
    {
        // Arrange
        var options = new GraphOptions
        {
            ClientId = "valid-client-id",
            ClientSecret = "valid-client-secret",
            TenantId = "valid-tenant-id",
            Scopes = ["scope1", "scope2"],
            IssuerIdentity = "valid-issuer-identity",
            DocumentNumberClaim = "extension_appid_DocumentNumber",
            DocumentTypeClaim = "extension_appid_DocumentType"
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void GraphOptions_MissingRequiredProperties_ShouldFailValidation()
    {
        // Arrange
        var options = new GraphOptions();

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GraphOptions.ClientId)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GraphOptions.ClientSecret)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GraphOptions.TenantId)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GraphOptions.IssuerIdentity)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GraphOptions.DocumentNumberClaim)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GraphOptions.DocumentTypeClaim)));
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}