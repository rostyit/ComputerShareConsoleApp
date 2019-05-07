using System.Collections.Generic;
using ComputerShare.Classes;

namespace ComputerShare.Services.Interfaces
{
    public interface IHtmlGeneratorService
    {
        string Generate(List<MapResult> mapResults);
    }
}