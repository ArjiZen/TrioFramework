using Bingosoft.Data;
using Bingosoft.Security;
using Bingosoft.Security.Principal;
using System.Linq;
using Bingosoft.TrioFramework.Models;

/// <summary>
/// 安全上下文扩展
/// </summary>
public static class SecurityContextExtension {
    private readonly static Dao _dao = Dao.Get();

    /// <summary>
    /// 根据 UserID获取指定人员
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="userid"></param>
    /// <returns></returns>
    public static IUser Get(this ISecurityProvider provider, string userid) {
        return _dao.QueryEntity<User>("framework.securitycontext.getuser", new { Id = userid });
    }

    /// <summary>
    /// 获取该部门的角色用户
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="roleName">角色名</param>
    /// <param name="orgName">部门名称</param>
    /// <returns></returns>
    public static User[] GetRoleUsers(this ISecurityProvider provider, string roleName, string orgName) {
        var list = _dao.QueryEntities<User>("framework.securitycontext.getroleuserbyorg", new { RoleName = roleName, OrgName = orgName });
        return list.ToArray();
    }

    /// <summary>
    /// 获取该部门的角色用户
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="roleName">角色名</param>
    /// <returns></returns>
    public static User[] GetRoleUsers(this ISecurityProvider provider, string roleName) {
        var list = _dao.QueryEntities<User>("framework.securitycontext.getroleuser", new { RoleName = roleName });
        return list.ToArray();
    }

    /// <summary>
    /// 判断当前用户是否在某个角色中
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roleName">角色名</param>
    /// <returns></returns>
    public static bool InRole(this IUser user, string roleName) {
        var isExists = _dao.QueryScalar<int>("framework.securitycontext.isinrole", new { UserId = user.Id, RoleName = roleName });
        return isExists > 0;
    }

    /// <summary>
    /// 判断用户是否属于某个部门
    /// </summary>
    /// <param name="user"></param>
    /// <param name="deptName"></param>
    /// <returns></returns>
    public static bool IsInDept(this IUser user, string deptName) {
        var isExists = _dao.QueryScalar<int>("framework.securitycontext.isindept", new { UserId = user.Id, DeptName = deptName });
        return isExists > 0;
    }

    /// <summary>
    /// 获取部门信息
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="id">部门id</param>
    /// <returns></returns>
    public static Organization GetOrganization(this ISecurityProvider provider, string id) {
        return Organization.Load(id);
    }

    /// <summary>
    /// 根据名称获取部门信息
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name">部门名称</param>
    /// <returns></returns>
    public static Organization GetOrganizationByName(this ISecurityProvider provider, string name) {
        return Organization.LoadByName(name);
    }
}
