using KanGainNET.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.IO;
using Xunit;

namespace KanGainNET.Tests
{
    public class GaleriaControllerTests : IDisposable
    {
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly string _tymczasowyFolderAplikacji;
        private readonly string _folderGalerii;

        public GaleriaControllerTests() //Tworzenie tymczasowego folderu dla testów i mockowanie tych folderow
        {
            _tymczasowyFolderAplikacji = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _folderGalerii = Path.Combine(_tymczasowyFolderAplikacji, "ZdjeciaGalerii");

            Directory.CreateDirectory(_folderGalerii);

            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.ContentRootPath).Returns(_tymczasowyFolderAplikacji);
        }

        [Fact]
        public void Zdjecie_GdyNazwaPusta_ZwracaNotFound()
        {
            var controller = new GaleriaController(_mockEnv.Object);

            var result = controller.Zdjecie(""); 

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Zdjecie_GdyPlikNieIstnieje_ZwracaNotFound()
        {
            var controller = new GaleriaController(_mockEnv.Object);

            var result = controller.Zdjecie("duzy_biceps.jpg"); 

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Zdjecie_ZabezpieczaPrzedHakerami_AtakPathTraversal()
        {
            var tajnyPlik = Path.Combine(_tymczasowyFolderAplikacji, "tajne_hasla.txt");
            File.WriteAllText(tajnyPlik, "super tajne dane serwera");

            var controller = new GaleriaController(_mockEnv.Object);

            var result = controller.Zdjecie("../tajne_hasla.txt");

            Assert.IsType<NotFoundResult>(result);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tymczasowyFolderAplikacji))
            {
                Directory.Delete(_tymczasowyFolderAplikacji, true);
            }
        }
    }
}