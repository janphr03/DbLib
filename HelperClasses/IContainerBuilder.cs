namespace HelperClasses;

public interface IContainerBuilder
{
    public Task<bool> imageExistsAsync(string imageName);
    public Task<bool> startContainerFromImageAsync(String imageName);
    void containerIstRunning(String containerName);
    void createImage(String imageName);
    void runCommand(String containerName, String command);

}