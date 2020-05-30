using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

#if WINDOWS_UWP && ENABLE_DOTNET
using Windows.Storage;
#endif

public class LM_ExperimentManager_OnlineStudy : MonoBehaviour
{
    [Min(1001)]
    public int firstSubjectId = 1001;
    public string azureConnectionString = string.Empty;
    public bool shuffleSceneOrder = true;
    public bool balanceConditionOrder = true;

    private Config config;
    private int thisSubjectID;
    private CloudStorageAccount azureAccount;
    private List<int> usedIds = new List<int>();

    async void Start()
    {
        // Get the config (dont use Config.Instance() as we need a preconfigured one)
        if (FindObjectOfType<Config>() != null)
        {
            config = Config.instance;
        }
        // Don't continue unless a config is found (even in editor)
        else
        {
            Debug.LogError("No Config found to autmatically configure");
            return;
        }



        // Are we using an Azure data repository?
        if (azureConnectionString != string.Empty)
        {
            // Get the Azure Storage client and blob information
            azureAccount = CloudStorageAccount.Parse(azureConnectionString);

            // Check Azure to see which ids have already been used; add them to our usedIds list
            await RetrieveAzureData();
        }

        

        // FIXME
        // ---------------------------------------------------------------------
        // Compute Subject ID (without overwriting data on the Azure storage client
        // ---------------------------------------------------------------------

        // start at the first possible subject id
        thisSubjectID = firstSubjectId;
        while (usedIds.Contains(thisSubjectID))
        {
            // increment the id we will use until it won't overwrite (next lowest possible id)
            thisSubjectID++;
        }

        // Put the subject ID into the config.subject field
        config.subject = thisSubjectID.ToString();


        // ---------------------------------------------------------------------
        // Counterbalance Conditions 
        // ---------------------------------------------------------------------
        if (balanceConditionOrder)
        {
            // create list of permutaitons (use permuation fucntions from LM_PermutedList.cs)
            var conditionList = LM_PermutedList.Permute(config.conditions, config.conditions.Count);
            List<string> theseConditions = new List<string>();

            int subCode;
            int.TryParse(config.subject, out subCode);
            subCode -= 1000;
            Debug.Log("subcode = " + subCode.ToString());
            // use the subject id multiples to determine condition order
            Debug.Log(conditionList.Count.ToString());
            for (int i = conditionList.Count; i > 0; i--)
            {
                // determine the highest multiple
                if (subCode % i == 0)
                {
                    Debug.Log(i.ToString());
                    // take this set of conditions based on the multiple used
                    foreach (var item in conditionList[i - 1])
                    {
                        Debug.Log(item.ToString());
                        theseConditions.Add(item);
                    }
                    break; // return control from this for loop
                }
            }
            config.conditions = theseConditions; // update the condition order
        }


        // ---------------------------------------------------------------------
        // Pseudo-Randomize the level/scene order
        // ---------------------------------------------------------------------
        if (shuffleSceneOrder)
        {
            var theseScenes = config.levelNames; // temporary variable
            LM_PermutedList.FisherYatesShuffle(theseScenes); // shuffle using function from LM_PermutedList.cs
            config.levelNames = theseScenes; // Update the level order
        }


        //----------------------------------------------------------------------
        // set up our config for the LM experiment
        //----------------------------------------------------------------------

        config.runMode = ConfigRunMode.NEW;
        config.bootstrapped = true;
        config.appPath = Application.persistentDataPath;
        DontDestroyOnLoad(config);

        //----------------------------------------------------------------------
        // Load the first level
        //----------------------------------------------------------------------
        Debug.Log(config.levelNames[config.levelNumber]);
        SceneManager.LoadScene(config.levelNames[config.levelNumber]);

    }

    public async Task RetrieveAzureData()
    {
        

        CloudBlobClient blobClient = azureAccount.CreateCloudBlobClient();
        // Access the folder where the data would be stored (create if does not exist)
        CloudBlobContainer container;
        container = blobClient.GetContainerReference(config.experiment);
        try
        {
            await container.CreateIfNotExistsAsync();
        }
        catch (StorageException)
        {

            Debug.Log("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
            throw;
        }

        // List all the blobs in the container
        Debug.Log("list all blobs in the container");
        BlobContinuationToken token = null;
        BlobResultSegment list = await container.ListBlobsSegmentedAsync(token);
        if (list.Results == null) Debug.Log("nothing stored yet");
        foreach (IListBlobItem blob in list.Results)
        {
            // Blob type will be CloudBlockBlob, CloudPageBlob or CloudBlobDirectory
            // Use blob.GetType() and cast to appropriate type to gain access to properties specific to each type

            int usedId;
            // check for all existing subject directories on the Azure Container
            int.TryParse(container.Uri.MakeRelativeUri(blob.Uri).ToString().Split('/')[1], out usedId);
            // Add them to our list to compare against the id to use
            usedIds.Add(usedId);
        }
    }

    
}
