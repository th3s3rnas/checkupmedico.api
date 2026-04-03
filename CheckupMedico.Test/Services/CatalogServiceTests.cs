namespace CheckupMedico.Test.Services;

using CheckupMedico.Application.Dto.Catalog;
using CheckupMedico.Application.Service.Catalog;
using CheckupMedico.Domain.Entity;
using CheckupMedico.Domain.Enum;
using CheckupMedico.Domain.Repository.Interface.LocalFile;
using Moq;

public class CatalogServiceTests
{
    [Fact]
    public void GetCampus_WhenHospitalsContainDuplicates_ReturnsDistinctCampuses()
    {
        var hospitalRepo = new Mock<IRepoLocalFileHospital>();
        var kitRepo = new Mock<IRepoLocalFileKit>();

        hospitalRepo.Setup(x => x.GetAll()).Returns(new List<HospitalEntity>
        {
            new() { Location = "MTY", Name = "H1", Campus = "C1", Email = "a@a", Responsible = "r", LocationDetails = "d", Area = "A", ContactDetails = "c", City = "Monterrey", State = "NL" },
            new() { Location = "MTY", Name = "H2", Campus = "C2", Email = "b@a", Responsible = "r", LocationDetails = "d", Area = "A", ContactDetails = "c", City = "Monterrey", State = "NL" },
            new() { Location = "QRO", Name = "H3", Campus = "C3", Email = "c@a", Responsible = "r", LocationDetails = "d", Area = "A", ContactDetails = "c", City = "Queretaro", State = "QT" }
        });

        var sut = new CatalogService(hospitalRepo.Object, kitRepo.Object);

        var result = sut.GetCampus();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Code == "MTY");
        Assert.Contains(result, x => x.Code == "QRO");
    }

    [Fact]
    public void GetHospitalsByCampus_WhenMatchingKitExists_ReturnsHospitalWithKit()
    {
        var hospitalRepo = new Mock<IRepoLocalFileHospital>();
        var kitRepo = new Mock<IRepoLocalFileKit>();

        hospitalRepo.Setup(x => x.GetByLocation("MTY")).Returns(new List<HospitalEntity>
        {
            new() { Location = "MTY", Name = "Hospital Uno", Campus = "Campus MTY", Email = "h1@test.com", Responsible = "Resp", LocationDetails = "Loc", Area = "Area", ContactDetails = "Contact", City = "Monterrey", State = "NL" }
        });

        kitRepo.Setup(x => x.GetAll()).Returns(new List<KitEntity>
        {
            new() { HospitalName = "Hospital Uno", Name = "KIT-001", Gender = SexEnum.Hombre, MinimumAge = 20, MaximumAge = 40 }
        });

        var sut = new CatalogService(hospitalRepo.Object, kitRepo.Object);
        var request = new HospitalsReqDto { CampusName = "MTY", Gender = SexEnum.Hombre };

        var birthDate = DateTime.UtcNow.Date.AddYears(-30);
        var result = sut.GetHospitalsByCampus(request, birthDate);

        Assert.Single(result);
        Assert.Equal("KIT-001", result[0].Kit);
        Assert.Equal("Hospital Uno", result[0].Name);
    }

    [Fact]
    public void GetHospitalsByCampus_WhenNoMatchingKit_ThrowsNotFoundException()
    {
        var hospitalRepo = new Mock<IRepoLocalFileHospital>();
        var kitRepo = new Mock<IRepoLocalFileKit>();

        hospitalRepo.Setup(x => x.GetByLocation("MTY")).Returns(new List<HospitalEntity>
        {
            new() { Location = "MTY", Name = "Hospital Uno", Campus = "Campus MTY", Email = "h1@test.com", Responsible = "Resp", LocationDetails = "Loc", Area = "Area", ContactDetails = "Contact", City = "Monterrey", State = "NL" }
        });

        kitRepo.Setup(x => x.GetAll()).Returns(new List<KitEntity>
        {
            new() { HospitalName = "Hospital Uno", Name = "KIT-001", Gender = SexEnum.Mujer, MinimumAge = 20, MaximumAge = 40 }
        });

        var sut = new CatalogService(hospitalRepo.Object, kitRepo.Object);
        var request = new HospitalsReqDto { CampusName = "MTY", Gender = SexEnum.Hombre };

        var birthDate = DateTime.UtcNow.Date.AddYears(-30);

        Assert.Throws<CheckupMedico.Transversal.Exception.NotFoundException>(
            () => sut.GetHospitalsByCampus(request, birthDate));
    }
}
