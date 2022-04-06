using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using TMPro;
using System.IO;

#if WINDOWS_UWP && ENABLE_DOTNET
using Windows.Storage;
#endif

///////////////////////////////////////////////////////////////////////////
///                                                                     ///
///                                                                     ///
/// DEPRECATED: Use LM_ExpStartup or LM_ExpStartupAzure.cs              ///
///                                                                     ///
///                                                                     ///
///////////////////////////////////////////////////////////////////////////

public class LM_ExperimentManager_OnlineStudy : MonoBehaviour
{

    [Min(1001)]
    public int firstSubjectId = 1001;
    public string azureConnectionString = string.Empty;
    public bool useAzureInEditor;
    //public bool shuffleSceneOrder = true;
    //public bool balanceConditionOrder = true;
    //[HideInInspector]
    public bool singleSceneBuild = true;
    public GameObject overrideWarning;
    public bool allowRetry;

    private Config config;
    private int thisSubjectID;
    private CloudStorageAccount azureAccount;
    private List<int> usedIds = new List<int>();

    private bool abortExperiment;

    async void Start()
    {

        // Get the config (dont use Config.Instance() as we need a preconfigured one)
        if (FindObjectOfType<Config>() != null)
        {
            config = Config.Instance;
        }
        // Don't continue unless a config is found (even in editor)
        else
        {
            Debug.LogError("No Config found to autmatically configure");
            return;
        }

        // Outside editor, don't run if any data exists for this experiment already (NO RETRY)
        // Unless in editor, don't let the app run a second time
        if (!allowRetry)
        {
            if (Directory.Exists(Application.persistentDataPath + "/" + config.experiment))
            {
                overrideWarning.SetActive(true);
                abortExperiment = true;
            }
            else Debug.Log("Can't find any files from a previous run; Good to go!");
        }


        // Are they okay to run?
        if (!abortExperiment)
        {
            // Are we using an Azure data repository?
            if (azureConnectionString != string.Empty)
            {
                // Get the Azure Storage client and blob information
                azureAccount = CloudStorageAccount.Parse(azureConnectionString);

                // Check Azure to see which ids have already been used; add them to our usedIds list
                await RetrieveAzureData();
            }

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


            // push a temporary hidden file to reserve that id on Azure
            if (azureConnectionString != string.Empty)
            {
                if (!Application.isEditor | useAzureInEditor)
                {
                    await PushAzureSubjectData();
                }

            }


            // ---------------------------------------------------------------------
            // Create all condition/scene pairwise comparisons 
            // ---------------------------------------------------------------------

            if (singleSceneBuild)
            {
                List<string[]> conditionSceneList = new List<string[]>();
                foreach (var condition in config.conditions)
                {
                    foreach (var scene in config.levelNames)
                    {
                        conditionSceneList.Add(new string[] { condition, scene });
                    }
                }

                // clear the condition and scene lists
                config.conditions.Clear();
                config.levelNames.Clear();



                // Use the subject ID to determine which condition/scene pair
                int subCode;
                int.TryParse(config.subject, out subCode);
                subCode -= firstSubjectId - 1; // remove the starting subject id value - 1
                Debug.Log("subcode = " + subCode.ToString());

                int conditionCode = subCode;
                while (conditionCode > conditionSceneList.Count) conditionCode -= conditionSceneList.Count;
                Debug.Log("Condition code: " + conditionCode + "/" + conditionSceneList.Count);

                // work through the multiples
                for (int i = conditionSceneList.Count; i > 0; i--)
                {
                    if (conditionCode % i == 0)
                    {
                        config.conditions.Add(conditionSceneList[i - 1][0]);
                        Debug.Log(conditionSceneList[i - 1][0]);
                        config.levelNames.Add(conditionSceneList[i - 1][1]);

                        break;
                    }
                }
            }
            else // if it's multiple scenes determine the condition and stick with that
            {
                // Parse the idNumber (string --> int)
                int subCode;
                int.TryParse(config.subject, out subCode);
                subCode -= firstSubjectId - 1; // remove the starting subject id value - 1
                Debug.Log("subcode = " + subCode.ToString());

                // reduce the subject code to be within the range of condition codes
                Debug.Log(config.conditions.Count.ToString() + " Conditions");
                //while (subCode > config.conditions.Count) subCode -= config.conditions.Count;
                Debug.Log("subcode = " + subCode.ToString());


                // determine the condition by checking remainder after division of subCode
                // relative to number of conditions
                string thisCondition = "undetermined";
                for (int i = config.conditions.Count; i > 0; i--)
                {
                    Debug.Log(i.ToString());
                    if (subCode % i == 0)
                    {
                        thisCondition = config.conditions[i - 1];
                        Debug.Log(thisCondition);

                        break;
                    }
                }

                // clear conditions and replace with the chosen condition for each scene
                config.conditions.Clear();
                foreach (var level in config.levelNames)
                {
                    config.conditions.Add(thisCondition);
                }
            }



            //// ---------------------------------------------------------------------
            //// Counterbalance Conditions 
            //// ---------------------------------------------------------------------
            //if (balanceConditionOrder)
            //{
            //    // create list of permutaitons (use permuation fucntions from LM_PermutedList.cs)
            //    var conditionList = LM_PermutedList.Permute(config.conditions, config.conditions.Count);
            //    List<string> theseConditions = new List<string>();

            //    // int subCode;
            //    int.TryParse(config.subject, out subCode);
            //    subCode -= 1000;
            //    Debug.Log("subcode = " + subCode.ToString());
            //    // use the subject id multiples to determine condition order
            //    Debug.Log(conditionList.Count.ToString());
            //    for (int i = conditionList.Count; i > 0; i--)
            //    {
            //        // determine the highest multiple
            //        if (subCode % i == 0)
            //        {
            //            Debug.Log(i.ToString());
            //            // take this set of conditions based on the multiple used
            //            foreach (var item in conditionList[i - 1])
            //            {
            //                Debug.Log(item.ToString());
            //                theseConditions.Add(item);
            //            }
            //            break; // return control from this for loop
            //        }
            //    }
            //    config.conditions = theseConditions; // update the condition order
            //}


            //// ---------------------------------------------------------------------
            //// Pseudo-Randomize the level/scene order
            //// ---------------------------------------------------------------------
            //if (shuffleSceneOrder)
            //{
            //    var theseScenes = config.levelNames; // temporary variable
            //    LM_PermutedList.FisherYatesShuffle(theseScenes); // shuffle using function from LM_PermutedList.cs
            //    config.levelNames = theseScenes; // Update the level order
            //}


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

            while (config.levelNames[config.levelNumber] == SceneManager.GetActiveScene().name)
            {
                Debug.LogWarning("The active scene is also the next scene in the queue");
                Debug.Log("Trying to skip and load the next scene");
                config.levelNumber++;

                if (config.levelNumber == config.levelNames.Count)
                {
                    Debug.Log("This is the only scene. Application will terminate after this scene.");
                    return;
                }
            }

            GameObject.Find("VerificationCode").GetComponent<TextMeshProUGUI>().text = config.conditions[config.levelNumber] + config.subject;

        }
    }

    private void Update()
    {

        if (!abortExperiment & Input.GetKeyDown(KeyCode.Return))
        {
            try
            {
                SceneManager.LoadScene(config.levelNames[config.levelNumber]);
            }
            catch (System.Exception)
            {
                Application.Quit();
            }
        }
        

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


    public async Task PushAzureSubjectData()
    {

        CloudBlobClient blobClient = azureAccount.CreateCloudBlobClient();
        // Access the folder where the data would be stored (create if does not exist)
        CloudBlobContainer container;
        container = blobClient.GetContainerReference(config.experiment);
        Debug.Log(container.Name);
        try
        {
            await container.CreateIfNotExistsAsync();
        }
        catch (StorageException)
        {

            Debug.Log("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
            throw;
        }

        // Upload BlockBlobs to the newly created container
        Debug.Log("2. Uploading Temporary BlockBlob(s) to reserve this id");

        // Create an empty placholder .txt file to upload (in persistent data path)
        StreamWriter placeholderFile = new StreamWriter(Application.persistentDataPath + "/.reserved");
        placeholderFile.Close();
        
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(config.subject + "/.reserved");
#if WINDOWS_UWP && ENABLE_DOTNET
		StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(Application.streamingAssetsPath.Replace('/', '\\'));
		StorageFile sf = await storageFolder.GetFileAsync(ImageToUpload);
		await blockBlob.UploadFromFileAsync(sf);
#else
        Debug.Log("Did our placeholder file write? " + File.Exists(Application.persistentDataPath + "/.reserved"));
        await blockBlob.UploadFromFileAsync(Application.persistentDataPath + "/.reserved");
#endif

    }
}
