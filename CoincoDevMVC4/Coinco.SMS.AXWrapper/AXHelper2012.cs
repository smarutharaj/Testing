﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.Dynamics.BusinessConnectorNet;
using System.Security.Principal;
using System.Reflection;
using System.Configuration;

namespace Coinco.SMS.AXWrapper
{
    public class AXHelper2012 : IAXHelper
    {
        static string axUserName = ConfigurationManager.AppSettings["AXUserName"].ToString();
        static string axPassword = ConfigurationManager.AppSettings["AXPassword"].ToString();
        string axCompany = ConfigurationManager.AppSettings["AXDefaultCompanyName"].ToString();

        //static string axUserName = "TBWIR";
        //static string axPassword = "Admin@123";
        //string axCompany = "CON";

        private System.Net.NetworkCredential networkCredentials = new System.Net.NetworkCredential(axUserName, axPassword);

        public DataTable GetDefaultSitesByUsername(string username)
        {
         
            object objUser, objInventId, objDefault, objSiteName;
            DataTable siteTable = new DataTable();
            Axapta ax = null;
            //string strCurrUserName = username.Split('\\')[1];
            try
            {
                ax = new Axapta();
             
                siteTable.Columns.Add("User", typeof(String));
                siteTable.Columns.Add("Sites", typeof(String));
                siteTable.Columns.Add("SitesName", typeof(String));
                siteTable.Columns.Add("Default", typeof(String));
                ax.LogonAs(username.Trim(), "", networkCredentials, axCompany, "", "", "");
                AxaptaRecord axRecord;
                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMASitesUser", username);



                axRecord.ExecuteStmt("select * from %1");
                // Loop through the set of retrieved records.

                while (axRecord.Found)
                {
                    objUser = axRecord.get_Field("NetworkAlias");
                    objInventId = axRecord.get_Field("InventSiteId");
                    objDefault = axRecord.get_Field("Default");
                    objSiteName = axRecord.get_Field("Name");
                    DataRow row = siteTable.NewRow();

                    row["User"] = objUser.ToString();
                    row["Sites"] = objInventId.ToString();
                    row["Default"] = objDefault.ToString();
                    row["SitesName"] = objSiteName.ToString();
                    siteTable.Rows.Add(row);

                    axRecord.Next();

                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }


            return siteTable;
        }

        //orderstatus - 0: Inprocess; 1-Posted; 2-Cancelled; -1: All
        public DataTable GetServiceOrders(string inventSiteId, string orderStatus, string userName)
        {

            Axapta ax = null;
            AxaptaRecord axRecord;
            DataTable serviceTable = new DataTable();
            serviceTable.Columns.Add("ServiceorderId", typeof(String));
            serviceTable.Columns.Add("CustAccount", typeof(String));
            serviceTable.Columns.Add("CustomerPO", typeof(String));
            serviceTable.Columns.Add("CustomerName", typeof(String));
            serviceTable.Columns.Add("Description", typeof(String));
            serviceTable.Columns.Add("Status", typeof(String));
            serviceTable.Columns.Add("WOClassification", typeof(String));
            serviceTable.Columns.Add("ServiceTechnician", typeof(String));
            serviceTable.Columns.Add("ServiceResponsible", typeof(String));
            serviceTable.Columns.Add("EntryDate", typeof(DateTime));
            try
            {
               
                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");

                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMAServiceOrders", inventSiteId, orderStatus);
                    axRecord.ExecuteStmt("select * from %1 order by %1.DateEntry desc");

                    while (axRecord.Found)
                    {
                        DataRow row = serviceTable.NewRow();
                        row["ServiceorderId"] = axRecord.get_Field("ServiceOrderId");
                        row["CustAccount"] = axRecord.get_Field("CustomerAccount");
                        row["CustomerPO"] = axRecord.get_Field("CustomerPO");
                        row["CustomerName"] = axRecord.get_Field("CustomerName");
                        row["Description"] = axRecord.get_Field("CustomerComments");
                        row["Status"] = axRecord.get_Field("Status");
                        row["WOClassification"] = axRecord.get_Field("WOClassification");
                        row["ServiceTechnician"] = axRecord.get_Field("ServiceTechnician");
                        row["ServiceResponsible"] = axRecord.get_Field("ServiceResponsible");
                        row["EntryDate"] = axRecord.get_Field("DateEntry");
                        serviceTable.Rows.Add(row);
                        axRecord.Next();
                    }

            }
            catch (Exception e)
            {
               // Take other error action as needed.
               throw e; 
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return serviceTable;
        }

        public DataTable GetServiceOrderLinesByServiceOrderId(string serviceOrderId, string userName)
        {

            Axapta ax = null;
            AxaptaRecord axRecord;
            DataTable serialTable = new DataTable();
            serialTable.Columns.Add("SerialNumber", typeof(String));
            serialTable.Columns.Add("PartNumber", typeof(String));
            serialTable.Columns.Add("PartType", typeof(String));
            serialTable.Columns.Add("Quantity", typeof(String));
            serialTable.Columns.Add("Warranty", typeof(String));
            serialTable.Columns.Add("RepairType", typeof(String));
            serialTable.Columns.Add("Comments", typeof(String));
            try
            {

                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");

                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMASerialNumberDetailsBySO", serviceOrderId);
                axRecord.ExecuteStmt("select * from %1");

                while (axRecord.Found)
                {
                    DataRow row = serialTable.NewRow();
                    row["SerialNumber"] = axRecord.get_Field("SerialNumber");
                    row["PartNumber"] = axRecord.get_Field("ItemID");
                    row["PartType"] = axRecord.get_Field("ItemGroup");
                    row["Quantity"] = axRecord.get_Field("Quantity");
                    row["Warranty"] = axRecord.get_Field("Warranty");
                    row["RepairType"] = axRecord.get_Field("RepairType");
                    row["Comments"] = axRecord.get_Field("Comments");
                    serialTable.Rows.Add(row);
                    axRecord.Next();
                }

            }
            catch (Exception e)
            {
                // Take other error action as needed.
                throw e;
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return serialTable;
        }

        public DataTable GetServiceOrderLinesBySerialNumberPartNumber(string serialId, string itemNumber, string custAccount, string userName)
        {

            Axapta ax = null;
            AxaptaRecord axRecord;
            DataTable serialTable = new DataTable();
            serialTable.Columns.Add("SerialNumber", typeof(String));
            serialTable.Columns.Add("PartNumber", typeof(String));
            serialTable.Columns.Add("PartType", typeof(String));
            serialTable.Columns.Add("Quantity", typeof(String));
            serialTable.Columns.Add("Warranty", typeof(String));
            serialTable.Columns.Add("RepairType", typeof(String));
            serialTable.Columns.Add("CustAccount", typeof(String));
            object[] param = new object[3];
            try
            {

                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                param[0] = serialId;
                param[1] = itemNumber;
                param[2] = custAccount;
                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMASerialNumberDetailsCust", param);
                axRecord.ExecuteStmt("select * from %1");

                while (axRecord.Found)
                {
                    DataRow row = serialTable.NewRow();
                    row["SerialNumber"] = axRecord.get_Field("SerialNumber");
                    row["PartNumber"] = axRecord.get_Field("ItemID");
                    row["PartType"] = axRecord.get_Field("ItemGroup");
                    row["Quantity"] = axRecord.get_Field("Quantity");
                    row["Warranty"] = axRecord.get_Field("Warranty");
                    row["RepairType"] = axRecord.get_Field("RepairType");
                    row["CustAccount"] = axRecord.get_Field("CustAccount");
                    serialTable.Rows.Add(row);
                    axRecord.Next();
                }




            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return serialTable;
        }

        public DataTable GetCustomerAddressList(string customerAccount, string userName)
        {


            Axapta ax = null;
            ax = new Axapta();
            DataTable addressTable = new DataTable();
            addressTable.Columns.Add("AddressID", typeof(String));
            addressTable.Columns.Add("AddressDesc", typeof(String));
            addressTable.Columns.Add("Address", typeof(String));
            addressTable.Columns.Add("IsBilling", typeof(String));
            addressTable.Columns.Add("IsShipping", typeof(String));
            try
            {
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                AxaptaRecord axRecord;
                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMACustomerAddresses", customerAccount);
                axRecord.ExecuteStmt("select * from %1");

                while (axRecord.Found)
                {
                    DataRow row = addressTable.NewRow();
                    row["AddressID"] = axRecord.get_Field("AddressID");
                    row["AddressDesc"] = axRecord.get_Field("AddressDesc");
                    row["Address"] = axRecord.get_Field("Address");
                    row["IsBilling"] = axRecord.get_Field("IsBilling");
                    row["IsShipping"] = axRecord.get_Field("Isshipping");
                    addressTable.Rows.Add(row);
                    axRecord.Next();

                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return addressTable;
        }

        public DataTable GetCustomers(string userName)
        {


            Axapta ax = null;

            ax = new Axapta();
            DataTable customerTable = new DataTable();
            customerTable.Columns.Add("CustomerAccount", typeof(String));
            customerTable.Columns.Add("CustomerName", typeof(String));
            try
            {
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                AxaptaRecord axRecord;
                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMACustomers");
                axRecord.ExecuteStmt("select * from %1");
                while (axRecord.Found)
                {
                    DataRow row = customerTable.NewRow();
                    row["CustomerAccount"] = axRecord.get_Field("CustAccount");
                    row["CustomerName"] = axRecord.get_Field("custName");
                    customerTable.Rows.Add(row);
                    axRecord.Next();

                }

            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return customerTable;
        }

        public DataTable GetWOClassificationList(string userName)
        {
            DataTable resultTable = new DataTable();
            Axapta ax = null;
            AxaptaRecord axRecord;
            try
            {
                // Login to Microsoft Dynamics AX.
                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                resultTable.Columns.Add("WOCode", typeof(String));
                resultTable.Columns.Add("WODescription", typeof(String));
                using (axRecord = ax.CreateAxaptaRecord("SMAWOClassification"))
                {
                    // Execute the query on the table.
                    axRecord.ExecuteStmt("select Code, Description from %1 where %1.DataAreaID=='" + axCompany + "'");
                    // Loop through the set of retrieved records.
                    while (axRecord.Found)
                    {

                        DataRow row = resultTable.NewRow();
                        row["WOCode"] = axRecord.get_Field("Code");
                        row["WODescription"] = axRecord.get_Field("Description");
                        resultTable.Rows.Add(row);
                        axRecord.Next();

                    }


                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return resultTable;
        }

        public DataTable GetTechnicians(string userName)
        {


            Axapta ax = null;

            ax = new Axapta();
            DataTable techniciansTable = new DataTable();
            techniciansTable.Columns.Add("Name", typeof(String));
            techniciansTable.Columns.Add("Number", typeof(String));
            try
            {
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                AxaptaRecord axRecord;
                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMATechnicians");
                axRecord.ExecuteStmt("select * from %1");


                while (axRecord.Found)
                {
                    DataRow row = techniciansTable.NewRow();
                    row["Name"] = axRecord.get_Field("Name");
                    row["Number"] = axRecord.get_Field("Number");
                    techniciansTable.Rows.Add(row);
                    axRecord.Next();

                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }

            return techniciansTable;
        }

       

        public DataTable GetItemNumbersList(string userName)
        {
            DataTable resultTable = new DataTable();
            Axapta ax = null;
            AxaptaRecord axRecord;
            try
            {
                // Login to Microsoft Dynamics AX.
                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                resultTable.Columns.Add("ItemNumber", typeof(String));
                resultTable.Columns.Add("ProductName", typeof(String));
                resultTable.Columns.Add("ProductSubType", typeof(String));
                using (axRecord = ax.CreateAxaptaRecord("InventTableExpanded"))
                {
                    // Execute the query on the table.
                    axRecord.ExecuteStmt("select ItemID, ProductName, ProductSubType  from %1 where %1.DataAreaID=='" + axCompany + "'");
                    // Loop through the set of retrieved records.
                    while (axRecord.Found)
                    {

                        DataRow row = resultTable.NewRow();
                        row["ItemNumber"] = axRecord.get_Field("ItemID");
                        row["ProductName"] = axRecord.get_Field("ProductName");
                        row["ProductSubType"] = axRecord.get_Field("ProductSubType");
                        resultTable.Rows.Add(row);
                        axRecord.Next();

                    }


                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return resultTable;
        }

        public string CreateServiceOrder(string siteId, string customerAccount, string AddressId, string CustomerPO, string ServiceTechnicianNo, string responsibleNo, string woClassification, string customerComments, string userName)
        {
           
            Axapta ax = null;
            object[] param = new object[8];
            string serviceOrderId;
            try
            {
                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");

                param[0] = siteId;
                param[1] = customerAccount;
                param[2] = AddressId;
                param[3] = CustomerPO;
                param[4] = ServiceTechnicianNo;
                param[5] = responsibleNo;
                param[6] = woClassification;
                param[7] = customerComments;
                serviceOrderId = ax.CallStaticClassMethod("ServiceOrderManagement", "createSMAServiceOrder", param).ToString();

                if (serviceOrderId == "")
                {
                    string parameterString = "";
                    for (int i = 0; i < param.Length; i++)
                    {
                        parameterString += "param[" + i + "]" + param[i].ToString() + "; ";
                    }

                    throw new Exception(String.Format("AX Failure:- Method='{0}' Parameters:Values = {1} - ", "createSMAServiceOrder", parameterString));
                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return serviceOrderId;
        }

        public bool CreateServiceOrderLinesList(string serviceOrderNo, string serialNumber, string partNumber, string partType, string quantity, string repairType, string warranty, string comments, string userName)
        {


            bool isSuccess = false;
            Axapta ax = null;

            object retval;
            try
            {
                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                bool flagValue;
                object[] param = new object[4];

                param[0] = serviceOrderNo;
                param[1] = partNumber;
                param[2] = serialNumber;
                param[3] = comments;

                retval = ax.CallStaticClassMethod("ServiceOrderManagement", "createSMAServiceObjectRelation", param).ToString();

                if (bool.TryParse(retval.ToString(), out flagValue))
                {
                    isSuccess = flagValue;
                }

                if (!isSuccess)
                {
                    string parameterString = "";
                    for (int i = 0; i < param.Length; i++)
                    {
                        parameterString += "param[" + i + "]" + param[i].ToString() + "; ";
                    }

                    throw new Exception(String.Format("AX Failure:- Method='{0}' Parameters:Values = {1} - ", "createSMAServiceObjectRelation", parameterString));
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return isSuccess;
        }
        #region "Service Order Process"

        // get Service Object Relation by Service Order for Service Order Process
        public DataTable GetServiceObjectRelationByServiceOrder(string serviceOrder, string userName)
        {

            Axapta ax = null;
            AxaptaRecord axRecord;
            DataTable serialTable = new DataTable();

            serialTable.Columns.Add("SORID", typeof(String));
            serialTable.Columns.Add("SerialNumber", typeof(String));

            try
            {

                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");

                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMASerialNumber", serviceOrder);
                axRecord.ExecuteStmt("select * from %1");

                while (axRecord.Found)
                {
                    DataRow row = serialTable.NewRow();
                    row["SORID"] = axRecord.get_Field("SORID");
                    row["SerialNumber"] = axRecord.get_Field("SerialNumber");
                    serialTable.Rows.Add(row);
                    axRecord.Next();
                }




            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return serialTable;
        }

        // Get Service Order Part lines by service order for service order process

        public DataTable GetServiceOrderPartLineByServiceOrder(string serviceOrderId, string userName)
        {

            Axapta ax = null;
            AxaptaRecord axRecord;
            DataTable serviceOrderLineTable = new DataTable();
            serviceOrderLineTable.Columns.Add("SerialNumber", typeof(String));
            serviceOrderLineTable.Columns.Add("SORelationID", typeof(String));
            serviceOrderLineTable.Columns.Add("TransactionType", typeof(String));
            serviceOrderLineTable.Columns.Add("Description", typeof(String));
            serviceOrderLineTable.Columns.Add("SpecialityCode", typeof(String));
            serviceOrderLineTable.Columns.Add("FailureCode", typeof(String));
            serviceOrderLineTable.Columns.Add("LineProperty", typeof(String));
            serviceOrderLineTable.Columns.Add("Qty", typeof(String));
            serviceOrderLineTable.Columns.Add("SalesPrice", typeof(String));
            serviceOrderLineTable.Columns.Add("Technician", typeof(String));
            serviceOrderLineTable.Columns.Add("ServiceComments", typeof(String));
            serviceOrderLineTable.Columns.Add("UniqueId", typeof(String));
            serviceOrderLineTable.Columns.Add("ItemNumber", typeof(String));
            serviceOrderLineTable.Columns.Add("Status", typeof(String));
            serviceOrderLineTable.Columns.Add("Site", typeof(String));
            serviceOrderLineTable.Columns.Add("WareHouse", typeof(String));
            serviceOrderLineTable.Columns.Add("Size", typeof(String));
            serviceOrderLineTable.Columns.Add("Color", typeof(String));
            serviceOrderLineTable.Columns.Add("Config", typeof(String));
            serviceOrderLineTable.Columns.Add("LocationId", typeof(String));
            serviceOrderLineTable.Columns.Add("TransSerialNumber", typeof(String));







            try
            {

                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");

                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMASOLineDetails", serviceOrderId);
                axRecord.ExecuteStmt("select * from %1 order by %1.UniqueID desc");

                while (axRecord.Found)
                {
                    DataRow row = serviceOrderLineTable.NewRow();
                    row["SerialNumber"] = axRecord.get_Field("SerialNumber");
                    row["SORelationID"] = axRecord.get_Field("ServiceObjectRelationId");
                    row["TransactionType"] = axRecord.get_Field("TransactionType");
                    row["Description"] = axRecord.get_Field("Description");
                    row["SpecialityCode"] = axRecord.get_Field("ProjCategoryId");
                    row["FailureCode"] = axRecord.get_Field("SMAFailureCode");
                    row["LineProperty"] = axRecord.get_Field("ProjLinePropertyId");
                    row["Qty"] = axRecord.get_Field("Qty");
                    row["SalesPrice"] = axRecord.get_Field("ProjSalesPrice");
                    row["Technician"] = axRecord.get_Field("WorkerName");
                    row["ServiceComments"] = axRecord.get_Field("DescriptionService");
                    row["ItemNumber"] = axRecord.get_Field("ItemId");
                    row["Status"] = axRecord.get_Field("ServiceOrderStatus");
                    row["UniqueId"] = axRecord.get_Field("UniqueID");
                    row["Site"] = axRecord.get_Field("InventSiteId");
                    row["WareHouse"] = axRecord.get_Field("InventLocationId");
                    row["Size"] = axRecord.get_Field("InventSizeId");
                    row["Color"] = axRecord.get_Field("InventColorId");
                    row["Config"] = axRecord.get_Field("configId");
                    row["LocationId"] = axRecord.get_Field("WMSLocationid");
                    row["TransSerialNumber"] = axRecord.get_Field("InventSerialId");

                    serviceOrderLineTable.Rows.Add(row);
                    axRecord.Next();
                }

            }

            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return serviceOrderLineTable;
        }



        //  Get technicians for service order process

        public DataTable GetTechniciansServiceOrderProcess(string transactionType, string specialityCode, string userName)
        {


            Axapta ax = null;

            ax = new Axapta();
            DataTable techniciansTable = new DataTable();
            techniciansTable.Columns.Add("Name", typeof(String));
            techniciansTable.Columns.Add("Number", typeof(String));
            try
            {
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                AxaptaRecord axRecord;
                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMATechniciansParts", transactionType, specialityCode);
                axRecord.ExecuteStmt("select * from %1");


                while (axRecord.Found)
                {
                    DataRow row = techniciansTable.NewRow();
                    row["Name"] = axRecord.get_Field("Name");
                    row["Number"] = axRecord.get_Field("Number");
                    techniciansTable.Rows.Add(row);
                    axRecord.Next();

                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }

            return techniciansTable;
        }

        // get Get Failure Code for service order process

        public DataTable GetFailureCodeList(string userName)
        {
            DataTable resultTable = new DataTable();
            Axapta ax = null;
            AxaptaRecord axRecord;
            try
            {
                // Login to Microsoft Dynamics AX.
                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                resultTable.Columns.Add("FailureCode", typeof(String));
                resultTable.Columns.Add("FailureDescription", typeof(String));
                using (axRecord = ax.CreateAxaptaRecord("SMAServiceTask"))
                {
                    // Execute the query on the table.
                    axRecord.ExecuteStmt("select ServiceTaskID, Description from %1 where %1.DataAreaID=='" + axCompany + "'");
                    // Loop through the set of retrieved records.
                    while (axRecord.Found)
                    {

                        DataRow row = resultTable.NewRow();
                        row["FailureCode"] = axRecord.get_Field("ServiceTaskID");
                        row["FailureDescription"] = axRecord.get_Field("Description");
                        resultTable.Rows.Add(row);
                        axRecord.Next();

                    }


                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return resultTable;
        }

        public DataTable GetLinePropertyList(string userName)
        {
            DataTable resultTable = new DataTable();
            Axapta ax = null;
            AxaptaRecord axRecord;
            try
            {
                // Login to Microsoft Dynamics AX.
                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                resultTable.Columns.Add("LinePropertyCode", typeof(String));
                resultTable.Columns.Add("LinePropertyName", typeof(String));
                using (axRecord = ax.CreateAxaptaRecord("ProjLineProperty"))
                {
                    // Execute the query on the table.
                    axRecord.ExecuteStmt("select LinePropertyID, Name from %1 where %1.DataAreaID=='" + axCompany + "'");
                    // Loop through the set of retrieved records.
                    while (axRecord.Found)
                    {

                        DataRow row = resultTable.NewRow();
                        row["LinePropertyCode"] = axRecord.get_Field("LinePropertyID");
                        row["LinePropertyName"] = axRecord.get_Field("Name");
                        resultTable.Rows.Add(row);
                        axRecord.Next();

                    }


                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return resultTable;
        }


        public DataTable GetSpecialityCodeList(string userName, string transactionId)
        {
            DataTable resultTable = new DataTable();
            Axapta ax = null;
            AxaptaRecord axRecord;
            try
            {
                // Login to Microsoft Dynamics AX.
                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");
                resultTable.Columns.Add("SpecialityCode", typeof(String));
                resultTable.Columns.Add("SpecialityDescription", typeof(String));
                using (axRecord = ax.CreateAxaptaRecord("CategoryTable"))
                {
                    // Execute the query on the table.e
                    //axRecord.ExecuteStmt("select CategoryID, CategoryName from %1 where %1.DataAreaID=='" + axCompany + "'");
                    axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMACategoryTable", transactionId);
                    axRecord.ExecuteStmt("select * from %1");


                    // Loop through the set of retrieved records.
                    while (axRecord.Found)
                    {

                        DataRow row = resultTable.NewRow();
                        row["SpecialityCode"] = axRecord.get_Field("CategoryId");
                        row["SpecialityDescription"] = axRecord.get_Field("CategoryName");
                        resultTable.Rows.Add(row);
                        axRecord.Next();

                    }


                }
            }
            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return resultTable;
        }
        #endregion

        #region "Sales Details"

        public DataTable GetSalesInformation(string salesSerialNumber, string userName)
        {

            Axapta ax = null;
            AxaptaRecord axRecord;
            DataTable salesTable = new DataTable();


            salesTable.Columns.Add("SalesNumber", typeof(String));
            salesTable.Columns.Add("InvoiceNumber", typeof(String));
            salesTable.Columns.Add("InvoiceDate", typeof(String));
            salesTable.Columns.Add("Name", typeof(String));
            salesTable.Columns.Add("ItemNumber", typeof(String));


            try
            {

                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");

                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMASalesInformation", salesSerialNumber);
                axRecord.ExecuteStmt("select * from %1");

                while (axRecord.Found)
                {
                    DataRow row = salesTable.NewRow();

                    row["SalesNumber"] = axRecord.get_Field("OrigSalesId");
                    row["InvoiceNumber"] = axRecord.get_Field("InvoiceId");
                    row["InvoiceDate"] = axRecord.get_Field("InvoiceDate");
                    row["Name"] = axRecord.get_Field("Name");
                    row["ItemNumber"] = axRecord.get_Field("ItemId");




                    salesTable.Rows.Add(row);
                    axRecord.Next();
                }

            }

            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return salesTable;
        }

        public DataTable GetSalesHistory(string salesSerialNumber, string userName)
        {

            Axapta ax = null;
            AxaptaRecord axRecord;
            DataTable salesTable = new DataTable();


            salesTable.Columns.Add("ServiceOrderId", typeof(String));
            salesTable.Columns.Add("SalesPrice", typeof(String));
            salesTable.Columns.Add("DateExecution", typeof(String));
            salesTable.Columns.Add("Description", typeof(String));



            try
            {

                ax = new Axapta();
                ax.LogonAs(userName.Trim(), "", networkCredentials, axCompany, "", "", "");

                axRecord = (AxaptaRecord)ax.CallStaticClassMethod("ServiceOrderManagement", "getSMAServiceHistory", salesSerialNumber);
                axRecord.ExecuteStmt("select * from %1");

                while (axRecord.Found)
                {
                    DataRow row = salesTable.NewRow();

                    row["ServiceOrderId"] = axRecord.get_Field("ServiceOrderId");
                    row["SalesPrice"] = axRecord.get_Field("ProjSalesPrice");
                    row["DateExecution"] = axRecord.get_Field("DateExecution");
                    row["Description"] = axRecord.get_Field("DescriptionService");





                    salesTable.Rows.Add(row);
                    axRecord.Next();
                }

            }

            catch (Exception e)
            {
                throw e;
                // Take other error action as needed.
            }
            finally
            {
                if (ax != null) ax.Logoff();
            }
            return salesTable;
        }

        #endregion
    }
}