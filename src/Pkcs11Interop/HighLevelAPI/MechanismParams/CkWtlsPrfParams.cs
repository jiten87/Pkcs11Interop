/*
 *  Pkcs11Interop - Open-source .NET wrapper for unmanaged PKCS#11 libraries
 *  Copyright (C) 2012 Jaroslav Imrich <jimrich(at)jimrich(dot)sk>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 3
 *  as published by the Free Software Foundation.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace Net.Pkcs11Interop.HighLevelAPI.MechanismParams
{
    /// <summary>
    /// Parameters for the CKM_WTLS_PRF mechanism
    /// </summary>
    public class CkWtlsPrfParams : IMechanismParams, IDisposable
    {
        /// <summary>
        /// Flag indicating whether instance has been disposed
        /// </summary>
        private bool _disposed = false;
        
        /// <summary>
        /// Low level mechanism parameters
        /// </summary>
        private LowLevelAPI.MechanismParams.CK_WTLS_PRF_PARAMS _lowLevelStruct = new LowLevelAPI.MechanismParams.CK_WTLS_PRF_PARAMS();
        
        /// <summary>
        /// Output of the operation
        /// </summary>
        public byte[] Output
        {
            get
            {
                int uintSize = LowLevelAPI.UnmanagedMemory.SizeOf(typeof(uint));
                byte[] outputLenBytes = LowLevelAPI.UnmanagedMemory.Read(_lowLevelStruct.OutputLen, uintSize);
                uint outputLen = Common.ConvertUtils.BytesToUint(outputLenBytes);
                return LowLevelAPI.UnmanagedMemory.Read(_lowLevelStruct.Output, (int)outputLen);
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the CkWtlsPrfParams class.
        /// </summary>
        /// <param name='digestMechanism'>Digest mechanism to be used (CKM)</param>
        /// <param name='seed'>Input seed</param>
        /// <param name='label'>Identifying label/param>
        /// <param name='outputLen'>Length in bytes that the output to be created shall have</param>
        public CkWtlsPrfParams(uint digestMechanism, byte[] seed, byte[] label, uint outputLen)
        {
            _lowLevelStruct.DigestMechanism = 0;
            _lowLevelStruct.Seed = IntPtr.Zero;
            _lowLevelStruct.SeedLen = 0;
            _lowLevelStruct.Label = IntPtr.Zero;
            _lowLevelStruct.LabelLen = 0;
            _lowLevelStruct.Output = IntPtr.Zero;
            _lowLevelStruct.OutputLen = IntPtr.Zero;

            _lowLevelStruct.DigestMechanism = digestMechanism;

            if (seed != null)
            {
                _lowLevelStruct.Seed = LowLevelAPI.UnmanagedMemory.Allocate(seed.Length);
                LowLevelAPI.UnmanagedMemory.Write(_lowLevelStruct.Seed, seed);
                _lowLevelStruct.SeedLen = (uint)seed.Length;
            }
            
            if (label != null)
            {
                _lowLevelStruct.Label = LowLevelAPI.UnmanagedMemory.Allocate(label.Length);
                LowLevelAPI.UnmanagedMemory.Write(_lowLevelStruct.Label, label);
                _lowLevelStruct.LabelLen = (uint)label.Length;
            }
            
            if (outputLen < 1)
                throw new ArgumentException("Value has to be positive number", "outputLen");
            
            _lowLevelStruct.Output = LowLevelAPI.UnmanagedMemory.Allocate((int)outputLen);
            
            byte[] outputLenBytes = Common.ConvertUtils.UintToBytes(outputLen);
            _lowLevelStruct.OutputLen = LowLevelAPI.UnmanagedMemory.Allocate(outputLenBytes.Length);
            LowLevelAPI.UnmanagedMemory.Write(_lowLevelStruct.OutputLen, outputLenBytes);
        }
        
        #region IMechanismParams
        
        /// <summary>
        /// Converts object to low level mechanism parameters
        /// </summary>
        /// <returns>Low level mechanism parameters</returns>
        public object ToLowLevelParams()
        {
            return _lowLevelStruct;
        }
        
        #endregion
        
        #region IDisposable
        
        /// <summary>
        /// Disposes object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Disposes object
        /// </summary>
        /// <param name="disposing">Flag indicating whether managed resources should be disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Dispose managed objects
                }
                
                // Dispose unmanaged objects
                LowLevelAPI.UnmanagedMemory.Free(ref _lowLevelStruct.Seed);
                _lowLevelStruct.SeedLen = 0;
                LowLevelAPI.UnmanagedMemory.Free(ref _lowLevelStruct.Label);
                _lowLevelStruct.LabelLen = 0;
                LowLevelAPI.UnmanagedMemory.Free(ref _lowLevelStruct.Output);
                LowLevelAPI.UnmanagedMemory.Free(ref _lowLevelStruct.OutputLen);
                
                _disposed = true;
            }
        }
        
        /// <summary>
        /// Class destructor that disposes object if caller forgot to do so
        /// </summary>
        ~CkWtlsPrfParams()
        {
            Dispose(false);
        }
        
        #endregion
    }
}
