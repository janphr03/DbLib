
using System;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using DocumentFormat.OpenXml.Spreadsheet;


namespace HelperClasses;

public class DockerContainerBuilder : IContainerBuilder
{
    
    private DockerClient _client; // 



    public async Task<bool> imageExistsAsync(string imageName)
    {
        
        _client ??= new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        
        try
        {
            var images = await _client.Images.ListImagesAsync(new ImagesListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["reference"] = new Dictionary<string, bool>
                    {
                        [imageName] = true
                    }
                }
            });

            return images.Any();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Prüfen des Images: {ex.Message}");
            return false;
        }
    }




    public async Task<bool> startContainerFromImageAsync(string imageName)
    {
         
        return false;
    }



    public void containerIstRunning(string containerName)
    {
        
    }


    public void createImage(string imageName)
    {
    }

    public void runCommand(string containerName, string command)
    {
    }
}