using FileMonitoringDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringServiceBLL
{
    public class ProjectBLL
    {
        ProjectDAL pDal = new ProjectDAL();

        /// <summary>
        /// 获得所有任务
        /// </summary>
        /// <returns></returns>
        public DataTable GetProject(string where)
        {
            return pDal.GetProject(where);
        }

        /// <summary>
        /// 根据任务编号获得任务ID
        /// <param name="taskNo"></param>
        /// </summary>
        /// <returns></returns>
        public string GetProjectIDByTaskNo(string taskNo)
        {
            return pDal.GetProjectIDByTaskNo(taskNo);
        }
        /*/// <summary>
        /// 是否已经设置了完成人
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="userNo">已经设置完成人的情况下，返回完成人的EMPLOYEENO</param>
        /// <returns></returns>
        public bool IsSetFinishedPerson(string projectID, out string userNo)
        {
            return pDal.IsSetFinishedPerson(projectID, out userNo);
        }
        */
        ///<summary>
        /// 判断当前任务的当前完成人是否设置
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <param name="employeeNo">员工编号</param>
        /// <returns>是否已经设置了当前员工为完成人</returns>
        public bool IsSetFinishedPerson2(string projectID, string employeeNo)
        {
            return pDal.IsSetFinishedPerson2(projectID, employeeNo);
        }

        /// <summary>
        /// 设置完成人
        /// </summary>
        /// <param name="userNo">员工号</param>
        /// <param name="projectID">任务ID</param>
        /// <returns></returns>
        public bool UpdateFinishedPerson(string userNo, string projectID)
        {
            return pDal.UpdateFinishedPerson(userNo, projectID);
        }

        /// <summary>
        /// 设置完成人（新）
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool SetFinishedPerson2(string projectID, string empNo)
        {
            return pDal.SetFinishedPerson2(projectID, empNo);
        }

        /// <summary>
        /// 设置完成状态为已完成
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <returns></returns>
        public bool UpdateFinishedStatus(string projectID)
        {
            return pDal.UpdateFinishedStatus(projectID);
        }

        /// <summary>
        /// 判断当前任务的修改记录是否存在
        /// </summary>
        /// <param name="taskNoOriginal">任务的原始文件夹名</param>
        /// <param name="modifyFolderName">修改文件夹名</param>
        /// <returns></returns>
        public bool IsExistProjectModify(string taskNoOriginal, string modifyFolderName)
        {
            return pDal.IsExistProjectModify(taskNoOriginal, modifyFolderName);
        }

        /// <summary>
        /// 根据任务编号判断任务是否存在于数据库中
        /// </summary>
        /// <param name="taskNo"></param>
        /// <returns></returns>
        public bool IsExistProjectByTaskNo(string taskNo)
        {
            return pDal.IsExistProjectByTaskNo(taskNo);
        }

        /// <summary>
        /// 添加修改记录
        /// </summary>
        /// <param name="taskNoOriginal">任务原始文件名</param>
        /// <param name="modifyFolderName">修改记录文件夹名</param>
        /// <returns></returns>
        public bool AddProjectModify(string taskNoOriginal, string modifyFolderName)
        {
            return pDal.AddProjectModify(taskNoOriginal, modifyFolderName);
        }

        /// <summary>
        /// 判断新的修改任务是否存在
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="modifyFolderName"></param>
        /// <returns></returns>
        public bool IsExistNewModifyScript(string projectID, string modifyFolderName)
        {
            return pDal.IsExistNewModifyScript(projectID, modifyFolderName);
        }

        /// <summary>
        /// 判断完成稿是否存在
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="modifyFolderName"></param>
        /// <returns></returns>
        public bool IsExistFinalModifyScript(string projectID, string modifyFolderName)
        {
            return pDal.IsExistFinalModifyScript(projectID, modifyFolderName);
        }

        /// <summary>
        /// 添加一条修改任务记录
        /// </summary>
        /// <param name="projectID">对应的普通任务的ID</param>
        /// <param name="folderName">目录名</param>
        /// <param name="isFinished">是否完成</param>
        /// <param name="reviewStatus">是否审核通过</param>
        /// <param name="dtCreate">创建时间</param>
        /// <returns></returns>
        public bool AddProjectModify(string projectID, string folderName, int isFinished, int reviewStatus, DateTime dtCreate)
        {
            return pDal.AddProjectModify(projectID, folderName, isFinished, reviewStatus, dtCreate);
        }

        /// <summary>
        /// 修改售后的完成状态
        /// </summary>
        /// <param name="taskNoOriginal"></param>
        /// <param name="modifyFolderName"></param>
        /// <returns></returns>
        public bool UpdateProjectModifyFinished(string taskNoOriginal, string modifyFolderName)
        {
            return pDal.UpdateProjectModifyFinished(taskNoOriginal, modifyFolderName);
        }

        /// <summary>
        /// 判断修改任务是否存在
        /// </summary>
        /// <param name="taskNo"></param>
        /// <param name="modifyName"></param>
        /// <param name="projectModifyID"></param>
        /// <param name="prjMdfFolder"></param>
        /// <param name="reviewStatus"></param>
        /// <returns></returns>
        public bool IsExistModifyTask(string taskNo, string modifyName, out string projectModifyID, /*out string prjMdfFolder,*/ out int reviewStatus)
        {
            return pDal.IsExistModifyTask(taskNo, modifyName, out projectModifyID, /*out prjMdfFolder,*/ out reviewStatus);
        }

        /// <summary>
        /// 获取尚未分配完成人的任务
        /// </summary>
        /// <returns></returns>
        public DataTable GetNotCreatedFolderTask(int isCreatedFolder)
        {
            return pDal.GetNotCreatedFolderTask(isCreatedFolder);
        }

        /// <summary>
        /// 设置“是否任务目录”值为已创建
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool SetIsCreatedFolder(string projectID)
        {
            return pDal.SetIsCreatedFolder(projectID);
        }

        /// <summary>
        /// 设置审核状态为通过
        /// </summary>
        /// <param name="projectModifyID"></param>
        /// <returns></returns>
        public bool SetReviewPass(string projectModifyID)
        {
            return pDal.SetReviewPass(projectModifyID);
        }

        /// <summary>
        /// 获取专业类别的configkey
        /// </summary>
        /// <returns></returns>
        public DataTable GetSpecialtyKey()
        {
            return pDal.GetSpecialtyKey();
        }

        /// <summary>
        /// 根据员工编号获取其任务目标值
        /// </summary>
        /// <returns></returns>
        public DataTable GetObjectiveValueByEmpNo()
        {
            return pDal.GetObjectiveValueByEmpNo();
        }

        /// <summary>
        /// 获取当前员工尚未完成的任务数（手里有几个活正在做）
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public int GetEmployeeCurrentTaskAmount(string empNo)
        {
            return pDal.GetEmployeeCurrentTaskAmount(empNo);
        }

        /// <summary>
        /// 根据专业类别码和员工编号获取数据
        /// </summary>
        /// <param name="spcConfigKey"></param>
        /// <param name="employeeNo"></param>
        /// <returns></returns>
        public DataTable GetSpecialtyBySpcIdAndEmpNo(string spcConfigKey, string employeeNo)
        {
            return pDal.GetSpecialtyBySpcIdAndEmpNo(spcConfigKey, employeeNo);
        }

        /// <summary>
        /// 获取当前专业的人数
        /// </summary>
        /// <param name="specialtyId">专业ID</param>
        /// <returns></returns>
        public DataTable GetEmpAmountBySpecialty(string specialtyId, out int empAmount)
        {
            return pDal.GetEmpAmountBySpecialty(specialtyId,out empAmount);
        }

        /// <summary>
        /// 获取任务的要求的交稿时间
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <param name="taskName">任务名称</param>
        /// <returns>返回完成人的数量</returns>
        public DataSet GetModifyTaskByPrjIdAndTaskName(string projectID, string taskName)
        {
            return pDal.GetModifyTaskByPrjIdAndTaskName(projectID, taskName);
        }
    }
}
