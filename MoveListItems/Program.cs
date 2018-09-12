using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace MoveListItems
{
    public class Program
    {
        public string script= "Export-SPWeb -Identity "https://crisp2.innova.com/test" -Path "C:\SPOnPremExport\" -ItemUrl "/test/SourceLib"  -NoFileCompression -IncludeVersions 4";
        public static void Main(string[] args)
        {
            RunScript(script);
            //MoveItems();
        }

        public static ClientContext GetContext(string siteUrl, string userName, string password, bool isO365)
        {
            ClientContext context = null;
            try
            {
                using (context = new ClientContext(siteUrl))
                    if (!string.IsNullOrEmpty(siteUrl) && !string.IsNullOrEmpty(userName))
                    {
                        if (isO365)
                        {
                            SecureString securePassword = new SecureString();
                            foreach (char c in password) securePassword.AppendChar(c);
                            context.Credentials = new SharePointOnlineCredentials(userName, securePassword);
                        }
                        else
                        {
                            context.Credentials = new NetworkCredential(userName, password);
                        }
                        context.Load(context.Web, w => w.Title, w => w.ServerRelativeUrl, w => w.Lists);
                        context.ExecuteQuery();
                    }
                    else
                        Console.WriteLine("User Name and Password missing. Please fill configuration file");
            }
            catch (Exception ex)
            {

                throw;
            }
            return context;
        }

        public static List GetList(ClientContext context, string listName)
        {
            try
            {
                if (context != null)
                {
                    List list = context.Web.Lists.GetByTitle(listName);
                    context.Load(list, w => w.Title);
                    context.ExecuteQuery();
                    return list;
                }

            }

            catch (Exception)
            {

                throw;
            }
            return null;
        }

        public static ListItemCollection GetListItems(ClientContext context, string listName)
        {
            ListItemCollection listCol = null;
            try
            {

                listCol = context.Web.Lists.GetByTitle(listName).GetItems(CamlQuery.CreateAllItemsQuery());
                context.Load(listCol);
                context.ExecuteQuery();
            }
            catch (Exception)
            {

                throw;
            }
            return listCol;
        }

        public static bool IsFieldExists(FieldCollection destFieldColl, string sourceFieldInternalName)
        {
            bool isExists = false;

            try
            {

                isExists = destFieldColl.AsEnumerable().Where(itm => itm.InternalName.Trim().ToUpper() == sourceFieldInternalName.Trim().ToUpper()).ToList().Count > 0 ? true : false;
            }
            catch (Exception)
            {

                throw;
            }
            return isExists;
        }

        public static void MoveItems()
        {
            try
            {
                ClientContext srcContext = GetContext(ConfigSettings.SourceSiteUrl, ConfigSettings.SourceUserName, ConfigSettings.SourcePassword, false);
                List sourceList = GetList(srcContext, ConfigSettings.SourceLibrary);
                ListItemCollection itemsToMove = GetListItems(srcContext, ConfigSettings.SourceLibrary);
                FieldCollection sourceFields = sourceList.Fields;
                srcContext.Load(sourceFields);
                srcContext.ExecuteQuery();

                ClientContext destContext = GetContext(ConfigSettings.DestinationSiteUrl, ConfigSettings.DestinationUserName, ConfigSettings.DestinationPassword, true);
                List destinationList = GetList(destContext, ConfigSettings.DestinationLibrary);
                FieldCollection destFieldColl = destinationList.Fields;
                destContext.Load(destFieldColl);
                destContext.ExecuteQuery();


                foreach (ListItem item in itemsToMove)
                {
                    if (item.FileSystemObjectType == FileSystemObjectType.File)
                    {
                        Microsoft.SharePoint.Client.File file = item.File;
                        FileVersionCollection versionCol = file.Versions;
                        srcContext.Load(versionCol);
                        srcContext.ExecuteQuery();

                        if (versionCol.Count > 0)
                        {
                            foreach (FileVersion version in versionCol)
                            {
                                Console.WriteLine(version.VersionLabel);
                            }
                        }


                        //file.MoveTo("https://magnusvistatech.sharepoint.com/sites/test/Shared%20Documents/", MoveOperations.Overwrite);
                        //srcContext.ExecuteQuery();
                    }
                }


                //foreach (Field _sField in sourceFields)
                //{
                //    if (!IsFieldExists(destFieldColl, _sField.InternalName))
                //    {
                //        destFieldColl.Add(_sField);
                //        destFieldColl.Context.Load(destFieldColl);
                //        destFieldColl.Context.ExecuteQuery();
                //    }
                //}


                //foreach (ListItem _sItem in itemsToMove)
                //{
                //    ListItem newItem = null;
                //    try
                //    {
                //        newItem = destinationList.GetItemById(_sItem.Id);
                //        if (newItem != null)
                //        {
                //            newItem = _sItem;
                //            newItem.Update();

                //        }
                //        else
                //        {
                //            ListItemCreationInformation newItemInfo = new ListItemCreationInformation();
                //            newItem = destinationList.AddItem(newItemInfo);
                //            newItem = _sItem;
                //            newItem.Update();
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        ListItemCreationInformation newItemInfo = new ListItemCreationInformation();
                //        newItem = destinationList.AddItem(newItemInfo);
                //        newItem = _sItem;
                //        newItem.Update();
                //    }
                //    destContext.Load(newItem);
                //}
                //destinationList.Update();
                //destContext.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string RunScript(string scriptText)
{

    Runspace runspace = RunspaceFactory.CreateRunspace();
    runspace.Open();

    Pipeline pipeline = runspace.CreatePipeline();
    pipeline.Commands.AddScript(scriptText);
    pipeline.Commands.Add("Out-String");

    Collection<psobject /> results = pipeline.Invoke();
    runspace.Close();

    // convert the script result into a single string

    StringBuilder stringBuilder = new StringBuilder();
    foreach (PSObject obj in results)
    {
        stringBuilder.AppendLine(obj.ToString());
    }

    return stringBuilder.ToString();
}
    }
}
