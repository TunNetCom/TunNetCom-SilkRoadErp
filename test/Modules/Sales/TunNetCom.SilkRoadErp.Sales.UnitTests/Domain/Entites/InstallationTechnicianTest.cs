using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class InstallationTechnicianTest
{
    [Fact]
    public void CreateInstallationTechnician_ShouldSetProperties()
    {
        var technician = InstallationTechnician.CreateInstallationTechnician(
            nom: "Ahmed Ben Ali",
            tel: "12345678",
            tel2: "87654321",
            tel3: "11111111",
            email: "ahmed@test.com",
            description: "Installateur certifié",
            photo: "/photos/ahmed.jpg");

        technician.Nom.Should().Be("Ahmed Ben Ali");
        technician.Tel.Should().Be("12345678");
        technician.Tel2.Should().Be("87654321");
        technician.Tel3.Should().Be("11111111");
        technician.Email.Should().Be("ahmed@test.com");
        technician.Description.Should().Be("Installateur certifié");
        technician.Photo.Should().Be("/photos/ahmed.jpg");
        technician.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void CreateInstallationTechnician_WithNullOptionals_ShouldKeepNull()
    {
        var technician = InstallationTechnician.CreateInstallationTechnician(
            nom: "Ahmed Ben Ali",
            tel: null,
            tel2: null,
            tel3: null,
            email: null,
            description: null,
            photo: null);

        technician.Tel.Should().BeNull();
        technician.Tel2.Should().BeNull();
        technician.Tel3.Should().BeNull();
        technician.Email.Should().BeNull();
        technician.Description.Should().BeNull();
        technician.Photo.Should().BeNull();
    }

    [Fact]
    public void UpdateInstallationTechnician_ShouldUpdateProperties()
    {
        var technician = InstallationTechnician.CreateInstallationTechnician(
            nom: "Ahmed",
            tel: null,
            tel2: null,
            tel3: null,
            email: null,
            description: null,
            photo: null);

        technician.UpdateInstallationTechnician(
            nom: "Sami",
            tel: "99999999",
            tel2: null,
            tel3: "88888888",
            email: "sami@test.com",
            description: "Nouvelle description",
            photo: null);

        technician.Nom.Should().Be("Sami");
        technician.Tel.Should().Be("99999999");
        technician.Tel2.Should().BeNull();
        technician.Tel3.Should().Be("88888888");
        technician.Email.Should().Be("sami@test.com");
        technician.Description.Should().Be("Nouvelle description");
        technician.Photo.Should().BeNull();
    }

    [Fact]
    public void SetId_ShouldUpdateId()
    {
        var technician = InstallationTechnician.CreateInstallationTechnician(
            nom: "Ahmed",
            tel: null,
            tel2: null,
            tel3: null,
            email: null,
            description: null,
            photo: null);

        technician.SetId(15);

        technician.Id.Should().Be(15);
    }
}
