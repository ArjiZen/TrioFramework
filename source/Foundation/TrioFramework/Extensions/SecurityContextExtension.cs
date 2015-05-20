using System.Linq;
using Bingosoft.Security;
using Bingosoft.Security.Principal;
using Bingosoft.TrioFramework;
using Bingosoft.TrioFramework.Models;

/// <summary>
/// 安全上下文扩展
/// </summary>
// ReSharper disable once CheckNamespace
public static class SecurityContextExtension
{
    /// <summary>
    /// 根据 loginid 或 userid 获取指定人员
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static IUser Get(this ISecurityProvider provider, string id)
    {
        var u = DBFactory.DB.QueryEntity<User>("trio.framework.securitycontext.getuser", new { Id = id });
        if (u == null)
        {
            return SecurityContext.Provider.GetUser(id);
        }
        return u;
    }

    /// <summary>
    /// 获取该部门的角色用户
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="roleName">角色名</param>
    /// <param name="orgName">部门名称</param>
    /// <returns></returns>
    public static User[] GetRoleUsers(this ISecurityProvider provider, string roleName, string orgName)
    {
        var list = DBFactory.DB.QueryEntities<User>("trio.framework.securitycontext.getroleuserbyorg", new { RoleName = roleName, OrgName = orgName });
        return list.ToArray();
    }

    /// <summary>
    /// 获取该部门的角色用户
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="roleName">角色名</param>
    /// <returns></returns>
    public static User[] GetRoleUsers(this ISecurityProvider provider, string roleName)
    {
        var list = DBFactory.DB.QueryEntities<User>("trio.framework.securitycontext.getroleuser", new { RoleName = roleName });
        return list.ToArray();
    }

    /// <summary>
    /// 判断当前用户是否在某个角色中
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roleName">角色名</param>
    /// <returns></returns>
    public static bool InRole(this IUser user, string roleName)
    {
        var isExists = DBFactory.DB.QueryScalar<int>("trio.framework.securitycontext.isinrole", new { UserId = user.Id, RoleName = roleName });
        return isExists > 0;
    }

    /// <summary>
    /// 判断用户是否属于某个部门
    /// </summary>
    /// <param name="user"></param>
    /// <param name="deptName"></param>
    /// <returns></returns>
    public static bool IsInDept(this IUser user, string deptName)
    {
        var isExists = DBFactory.DB.QueryScalar<int>("trio.framework.securitycontext.isindept", new { UserId = user.Id, DeptName = deptName });
        return isExists > 0;
    }

    /// <summary>
    /// 获取部门信息
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="id">部门id</param>
    /// <returns></returns>
    public static Organization GetOrganization(this ISecurityProvider provider, string id)
    {
        return Organization.Load(id);
    }

    /// <summary>
    /// 根据名称获取部门信息
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name">部门名称</param>
    /// <returns></returns>
    public static Organization GetOrganizationByName(this ISecurityProvider provider, string name)
    {
        return Organization.LoadByName(name);
    }
}
