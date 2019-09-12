﻿using Ctrl.Plugin.PayCore.Request;
using System.Text;

namespace Ctrl.Plugin.Wx.Response
{
    public class FundFlowDownloadResponse : BaseResponse
    {
        /// <summary>
        /// 获取账单文件
        /// </summary>
        public byte[] GetBillFile()
        {
            return _billFile;
        }

        private byte[] _billFile;

        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
            if (!string.IsNullOrEmpty(Raw))
            {
                _billFile = Encoding.UTF8.GetBytes(Raw);
            }
        }
    }
}
