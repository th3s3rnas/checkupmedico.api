namespace CheckupMedico.Test.Services;

using CheckupMedico.Application.Doc.Interface;
using CheckupMedico.Application.Dto.Catalog;
using CheckupMedico.Application.Dto.Checkup;
using CheckupMedico.Application.Service.Checkup;
using CheckupMedico.Domain.Entity;
using CheckupMedico.Domain.Repository.Interface.LocalFile;
using Moq;

public class CheckupServiceTests
{
    [Fact]
    public void Create_WhenBillingConfigIsMissing_ThrowsInvalidOperationException()
    {
        var billingRepo = new Mock<IRepoLocalFileBillingConfig>();
        var doc = new Mock<ICheckupITESMDoc>();

        billingRepo.Setup(x => x.GetAll()).Returns(new List<BillingConfigEntity>());

        var sut = new CheckupService(billingRepo.Object, doc.Object);

        Assert.Throws<InvalidOperationException>(() =>
            sut.Create(new HospitalListDto(), "A1", "Jane Doe", new DateTime(1990, 1, 1)));
    }

    [Fact]
    public void Create_WhenInputIsValid_BuildsDocumentAndReturnsStream()
    {
        var billingRepo = new Mock<IRepoLocalFileBillingConfig>();
        var doc = new Mock<ICheckupITESMDoc>();

        billingRepo.Setup(x => x.GetAll()).Returns(new List<BillingConfigEntity>
        {
            new() { Institute = "ITESM", FullName = "Instituto", Number = "1", Rfc = "RFC" }
        });

        CheckupITESMDto? captured = null;

        doc.Setup(x => x.Build(It.IsAny<CheckupITESMDto>()))
            .Callback<CheckupITESMDto>(dto => captured = dto);
        doc.Setup(x => x.GetDoc()).Returns(new byte[] { 1, 2, 3, 4 });

        var sut = new CheckupService(billingRepo.Object, doc.Object);

        var stream = sut.Create(new HospitalListDto
        {
            City = "Monterrey",
            State = "NL",
            Name = "Hospital Uno",
            Campus = "Campus",
            Responsible = "Responsable",
            Kit = "KIT-001",
            LocationDetails = "Piso 2",
            ContactDetails = "Contacto",
            Email = "mail@test.com"
        }, "A123", "Jane Doe", new DateTime(1990, 1, 1));

        Assert.NotNull(stream);
        Assert.Equal(4, stream.Length);
        Assert.NotNull(captured);
        Assert.Equal("ITESM", captured!.Institute);
        Assert.Equal("A123", captured.PayrollId);
        Assert.Equal("Instituto", captured.FullName);
    }
}
