using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using UnityEngine.UI;

#if WINDOWS_UWP && ENABLE_DOTNET
using Windows.Storage;
#endif

public class LM_AzureStorage : MonoBehaviour
{
    public string connectionString = string.Empty;
	public bool useInEditor;
	public List<string> additionalSaveFiles; // save additional files from the datapath not logged automatically by LM

	private Experiment experiment;
	private Text azureMessage;

    protected CloudStorageAccount storageAccount;

    // Start is called before the first frame update
    void Start()
    {
		Debug.Log("configuring Azure storage");
        storageAccount = CloudStorageAccount.Parse(connectionString);
		experiment = FindObjectOfType<Experiment>();

		azureMessage = gameObject.AddComponent<Text>();
	}


	public async Task BasicStorageBlockBlobOperationsAsync()
	{
		// temporarily override the hud to provide save/load messages via hud
		var hud = experiment.player.GetComponentInChildren<HUD>();
		var oldHudMessage = hud.GetMessage();
		var oldHudDuration = hud.SecondsToShow;
		hud.SecondsToShow = 300; // show for 5 minutes max
		azureMessage.text = "Transferring data to cloud servers... this may take some time. \n\n" +
                            "--DO NOT CLOSE THE APPLICATION-- \n\n" +
                            "If more than 5 minutes have passed, manually (force) \n" +
                            "quit the application and contact the experimenter";
		hud.setMessage((string) azureMessage.text);
		hud.ForceShowMessage();



		// Create a blob client for interacting with the blob service.
		CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

		// Create a container for organizing blobs within the storage account.
		Debug.Log("1. Creating Container");
		CloudBlobContainer container = blobClient.GetContainerReference(experiment.config.experiment);
		try
		{
			await container.CreateIfNotExistsAsync();
		}
		catch (StorageException)
		{
			Debug.Log("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
			throw;
		}

		// To view the uploaded blob in a browser, you have two options. The first option is to use a Shared Access Signature (SAS) token to delegate 
		// access to the resource. See the documentation links at the top for more information on SAS. The second approach is to set permissions 
		// to allow public access to blobs in this container. Uncomment the line below to use this approach. Then you can view the image 
		// using: https://[InsertYourStorageAccountNameHere].blob.core.windows.net/democontainer/HelloWorld.png
		// await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

		// Upload BlockBlobs to the newly created container
		Debug.Log("2. Uploading BlockBlob(s)");

        // Upload the Landmarks log files
		CloudBlockBlob blockBlob = container.GetBlockBlobReference(experiment.config.subject + "/" + System.DateTime.Now.ToString("yyMMddHHmmss") + "_" + experiment.logfile);


#if WINDOWS_UWP && ENABLE_DOTNET
		StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(Application.streamingAssetsPath.Replace('/', '\\'));
		StorageFile sf = await storageFolder.GetFileAsync(ImageToUpload);
		await blockBlob.UploadFromFileAsync(sf);
#else
		Debug.Log(experiment.dataPath + experiment.logfile);
		await blockBlob.UploadFromFileAsync(experiment.dataPath + experiment.logfile);
#endif


        CloudBlockBlob[] fileBlobs = new CloudBlockBlob[additionalSaveFiles.Count];
        // Try to upload any other files the user asked to be saved
        for (int i = 0; i < additionalSaveFiles.Count; i++)
		{
			fileBlobs[i] = container.GetBlockBlobReference(experiment.config.subject + "/" + System.DateTime.Now.ToString("yyMMddHHmmss") + "_" + additionalSaveFiles[i]);
			
			try
			{
#if WINDOWS_UWP && ENABLE_DOTNET
		StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(Application.streamingAssetsPath.Replace('/', '\\'));
		StorageFile sf = await storageFolder.GetFileAsync(ImageToUpload);
		await blockBlob.UploadFromFileAsync(sf);
#else
				Debug.Log(experiment.dataPath + additionalSaveFiles[i]);
				await fileBlobs[i].UploadFromFileAsync(experiment.dataPath + additionalSaveFiles[i]);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
				Debug.LogError("Error saving file " + experiment.dataPath + additionalSaveFiles[i]+ "; File does not exist or cannot be read.");
			}
		}

#endif

		//		// List all the blobs in the container 
		//		Debug.Log("3. List Blobs in Container");
		//		BlobContinuationToken token = null;
		//		BlobResultSegment list = await container.ListBlobsSegmentedAsync(token);
		//		foreach (IListBlobItem blob in list.Results)
		//		{
		//			// Blob type will be CloudBlockBlob, CloudPageBlob or CloudBlobDirectory
		//			// Use blob.GetType() and cast to appropriate type to gain access to properties specific to each type
		//			Debug.Log(string.Format("- {0} (type: {1})", blob.Uri, blob.GetType()));
		//		}

		//		// Download a blob to your file system
		//		string path;
		//		Debug.Log(string.Format("4. Download Blob from {0}", blockBlob.Uri.AbsoluteUri));
		//		string fileName = string.Format("CopyOf{0}", ImageToUpload);

		//#if WINDOWS_UWP && ENABLE_DOTNET
		//		storageFolder = ApplicationData.Current.TemporaryFolder;
		//		sf = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
		//		path = sf.Path;
		//		await blockBlob.DownloadToFileAsync(sf);
		//#else
		//		path = Path.Combine(Application.temporaryCachePath, fileName);
		//		await blockBlob.DownloadToFileAsync(path, FileMode.Create);
		//#endif

		//		Debug.Log("File " + filename.ToString() + " written to " + path);

		//		// Clean up after the demo 
		//		//WriteLine("5. Delete block Blob");
		//		//await blockBlob.DeleteAsync();

		//		// When you delete a container it could take several seconds before you can recreate a container with the same
		//		// name - hence to enable you to run the demo in quick succession the container is not deleted. If you want 
		//		// to delete the container uncomment the line of code below. 
		//		//WriteLine("6. Delete Container -- Note that it will take a few seconds before you can recreate a container with the same name");
		//		//await container.DeleteAsync();

		Debug.Log("Azure Process Complete");

		hud.SecondsToShow = oldHudDuration;
		hud.setMessage(oldHudMessage);
		hud.ForceShowMessage();
	}
}
