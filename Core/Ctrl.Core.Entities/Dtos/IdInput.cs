using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Core.Entities.Dtos
{
    /// <summary>
    ///     以传递一个实体Id值给应用服务方法。
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class IdInput<TId> : IInputDto
    {
        public TId Id { get; set; }

        public IdInput() { }

        public IdInput(TId id) {
            Id = id;
        }
    }
    /// <summary>
    ///     Id类型为一个String 类型的快捷实现
    /// </summary>
    public class IdInput:IdInput<string>
    {
        public IdInput() { }

        public IdInput(string id) : base(id) { }
    }
}
