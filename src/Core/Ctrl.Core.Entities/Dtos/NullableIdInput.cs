using System;

namespace Ctrl.Core.Entities.Dtos
{
    /// <summary>
    ///     传递一个可为空的id给服务方法
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class NullableIdInput<TId>:IInputDto where TId :struct
    {
        /// <summary>
        ///     唯一标识
        /// </summary>
        /// <value></value>
        public TId? Id { get; set; }
        /// <summary>
        ///     构造函数
        /// </summary>
        public NullableIdInput(){}
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="id"></param>
        public NullableIdInput(TId? id){

        }

    }
    /// <summary>
    ///     id类型为Guid的一个快捷方式
    /// </summary>
    /// <typeparam name="Guid"></typeparam>
    public class NullableIdInput:NullableIdInput<Guid>
    {   
        /// <summary>
        ///     构造函数
        /// </summary>
        public NullableIdInput(){}
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="id"></param>
        public NullableIdInput(Guid? id):base(id){
            
        }
    }
}