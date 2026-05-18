import React, { useState } from 'react'
import { Shield, CreditCard, Lock, CheckCircle } from 'lucide-react'
import { toast } from 'react-hot-toast'

// Meta Cloud API Configuration
const META_ACCESS_TOKEN = "EAAYwyuDN7kkBRfNmFJKnNjrqeWaFVjZBv4EI73A4vbrTZBEGsxmW1J65lv9AuV0OjPNzJszvqx6vj8xN5N1DEnCHS3bdGM88b1zZAi3HT05PUynhGTOl8KdsAUrULVXdbypaZAPMdpjC7pAiiZBpbwfdwlUkvvBhHKkglldrF4QqMlXehDIWubg7kkF8wSZCnBv2NtyPmeSclKd9N5UwVzSOq9ZCPgv5ZBCEhGMT6BZBnjgadoweDqZBo5YilLZAZBnmfwzXLkO8d6FvetqOvtHMehGFHgfk";
const PHONE_NUMBER_ID = "1015976348275183";
const API_VERSION = "v25.0";

interface CardPaymentProps {
  amount: number
  onSuccess: () => void
  onCancel: () => void
}

export const CardPayment: React.FC<CardPaymentProps> = ({ amount, onSuccess, onCancel }) => {
  const [cardNumber, setCardNumber] = useState('')
  const [expiry, setExpiry] = useState('')
  const [cvv, setCvv] = useState('')
  const [phone, setPhone] = useState('')
  const [name, setName] = useState('')
  const [isProcessing, setIsProcessing] = useState(false)
  const [error, setError] = useState('')
  const [step, setStep] = useState<'form' | 'otp'>('form')
  const [otp, setOtp] = useState('')
  const [generatedOtp, setGeneratedOtp] = useState<string>('')
  const [otpExpiry, setOtpExpiry] = useState<number>(0)
  const [timeLeft, setTimeLeft] = useState<number>(0)

  // Countdown timer for OTP
  React.useEffect(() => {
    if (step === 'otp' && otpExpiry > 0) {
      const interval = setInterval(() => {
        const remaining = Math.max(0, Math.floor((otpExpiry - Date.now()) / 1000))
        setTimeLeft(remaining)
        if (remaining === 0) {
          clearInterval(interval)
        }
      }, 1000)
      return () => clearInterval(interval)
    }
  }, [step, otpExpiry])

  const sendWhatsAppOtp = async (targetPhone: string, otpCode: string) => {
    // Basic phone normalization (removes spaces, dashes, etc. and ensures + prefix)
    const normalizedPhone = targetPhone.replace(/\D/g, '');
    
    try {
      const url = `https://graph.facebook.com/${API_VERSION}/${PHONE_NUMBER_ID}/messages`;
      const response = await fetch(url, {
        method: "POST",
        headers: {
          "Authorization": `Bearer ${META_ACCESS_TOKEN}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          messaging_product: "whatsapp",
          to: normalizedPhone,
          type: "template",
          template: {
            name: "hello_world", // Sandbox default template
            language: { code: "en_US" },
          },
        }),
      });
      const data = await response.json();
      console.log("WhatsApp API Response:", data);
      
      if (data.error) {
        toast.error(`WhatsApp Error: ${data.error.message}`);
      } else {
        toast.success("OTP sent to WhatsApp!");
      }
    } catch (error) {
      console.error("WhatsApp Sending Error:", error);
      toast.error("Failed to send WhatsApp message.");
    }
  };

  const generateAndSendOtp = () => {
    const newOtp = Math.floor(100000 + Math.random() * 900000).toString()
    setGeneratedOtp(newOtp)
    setOtpExpiry(Date.now() + 120000) // 2 minutes TTL
    setTimeLeft(120)
    
    // Call real WhatsApp API
    if (phone) {
      sendWhatsAppOtp(phone, newOtp);
    }
    
    // Simulate incoming SMS (Keeping this as fallback/UI visual)
    toast.custom((t) => (
      <div className={`${t.visible ? 'animate-enter' : 'animate-leave'} max-w-md w-full bg-white shadow-lg rounded-lg pointer-events-auto flex ring-1 ring-black ring-opacity-5`}>
        <div className="flex-1 w-0 p-4">
          <div className="flex items-start">
            <div className="ml-3 flex-1">
              <p className="text-sm font-bold text-gray-900">📱 New Message</p>
              <p className="mt-1 text-sm text-gray-500">
                GreenPantry: Your OTP is <span className="font-bold text-black">{newOtp}</span> for INR {amount.toFixed(2)}. Valid for 2 mins. Do not share.
              </p>
            </div>
          </div>
        </div>
      </div>
    ), { duration: 10000, position: 'top-right' })
  }

  // MNC Level Validation: Luhn Algorithm
  const luhnCheck = (val: string) => {
    let sum = 0
    for (let i = 0; i < val.length; i++) {
      let intVal = parseInt(val.substr(i, 1))
      if (i % 2 === 0) {
        intVal *= 2
        if (intVal > 9) {
          intVal = 1 + (intVal % 10)
        }
      }
      sum += intVal
    }
    return sum % 10 === 0
  }

  const validateCard = () => {
    const cleanNum = cardNumber.replace(/\s+/g, '')
    if (cleanNum.length !== 16) return 'Card number must be 16 digits'
    if (!luhnCheck(cleanNum)) return 'Invalid card number checksum'
    
    if (!/^(0[1-9]|1[0-2])\/([0-9]{2})$/.test(expiry)) return 'Invalid expiry format (MM/YY)'
    const [month, year] = expiry.split('/')
    const expDate = new Date(2000 + parseInt(year), parseInt(month) - 1)
    if (expDate < new Date()) return 'Card has expired'
    
    if (!/^[0-9]{3,4}$/.test(cvv)) return 'Invalid CVV'
    if (name.trim().length < 3) return 'Please enter name on card'
    if (!/^\+?[1-9]\d{9,14}$/.test(phone.replace(/\D/g, ''))) return 'Please enter a valid phone number with country code'
    
    return null
  }

  const handlePay = () => {
    const err = validateCard()
    if (err) {
      setError(err)
      toast.error(err)
      return
    }

    setError('')
    setIsProcessing(true)

    // Simulate connecting to bank ACS page
    setTimeout(() => {
      setIsProcessing(false)
      setStep('otp')
      generateAndSendOtp()
      toast.success('Redirecting to bank for 3D Secure verification')
    }, 2500)
  }

  const handleVerifyOtp = () => {
    if (Date.now() > otpExpiry) {
      setError('This OTP has expired. Please request a new one.')
      return
    }
    if (otp !== generatedOtp) {
      setError('Incorrect OTP. Please try again.')
      return
    }
    setError('')
    setIsProcessing(true)
    setTimeout(() => {
      setIsProcessing(false)
      toast.success('Secure payment authorized!')
      onSuccess()
    }, 2000)
  }

  if (step === 'otp') {
    return (
      <div className="bg-white rounded-lg p-8 max-w-md mx-auto border border-gray-200 shadow-2xl relative overflow-hidden">
        {/* Bank Logo / Header */}
        <div className="text-center mb-6">
          <div className="mx-auto w-16 h-16 bg-blue-50 rounded-full flex items-center justify-center mb-4">
            <Shield className="w-8 h-8 text-blue-600" />
          </div>
          <h3 className="text-xl font-bold text-gray-900">Bank Authentication</h3>
          <p className="text-sm text-gray-500 mt-2">
            To protect your card against unauthorized use, we have sent a One Time Password (OTP) to your registered mobile number ending in ****{cardNumber.replace(/\s+/g, '').slice(-4) || '1234'}.
          </p>
        </div>

        <div className="bg-gray-50 p-4 rounded-lg mb-6 border border-gray-100">
          <div className="flex justify-between mb-2">
            <span className="text-sm text-gray-600">Merchant:</span>
            <span className="text-sm font-semibold text-gray-900">GreenPantry</span>
          </div>
          <div className="flex justify-between mb-2">
            <span className="text-sm text-gray-600">Amount:</span>
            <span className="text-sm font-semibold text-gray-900">₹{amount.toFixed(2)}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-sm text-gray-600">Card Number:</span>
            <span className="text-sm font-semibold text-gray-900">XXXX-XXXX-XXXX-{cardNumber.replace(/\s+/g, '').slice(-4)}</span>
          </div>
        </div>

        {error && (
          <div className="bg-red-50 text-red-600 text-sm p-3 rounded-lg mb-4 text-center">
            {error}
          </div>
        )}

        <div className="mb-6">
          <div className="flex justify-between items-end mb-2">
            <label className="block text-sm font-medium text-gray-700">Enter OTP</label>
            <span className={`text-xs font-bold ${timeLeft > 0 ? 'text-blue-600' : 'text-red-600'}`}>
              {timeLeft > 0 
                ? `Expires in ${Math.floor(timeLeft / 60)}:${(timeLeft % 60).toString().padStart(2, '0')}` 
                : 'Expired'}
            </span>
          </div>
          <input
            type="text"
            placeholder="123456"
            maxLength={6}
            value={otp}
            onChange={(e) => {
              setOtp(e.target.value.replace(/\D/g, ''))
              setError('')
            }}
            disabled={timeLeft === 0}
            className={`w-full text-center px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 text-2xl tracking-widest font-mono ${timeLeft === 0 ? 'bg-gray-100 border-gray-200 text-gray-400' : 'border-gray-300'}`}
          />
        </div>

        <button
          onClick={handleVerifyOtp}
          disabled={isProcessing || otp.length !== 6 || timeLeft === 0}
          className="w-full px-4 py-3 bg-blue-600 text-white font-bold rounded-lg hover:bg-blue-700 disabled:opacity-70 flex items-center justify-center space-x-2 transition-all"
        >
          {isProcessing ? (
            <span className="animate-spin border-2 border-white border-t-transparent rounded-full w-5 h-5"></span>
          ) : (
            <Lock className="w-5 h-5" />
          )}
          <span>{isProcessing ? 'Verifying...' : 'Submit'}</span>
        </button>

        <div className="mt-6 flex items-center justify-between text-xs text-gray-500">
          <button 
            onClick={() => {
              setOtp('')
              setError('')
              generateAndSendOtp()
            }} 
            className="hover:text-blue-600 font-medium"
          >
            Resend OTP
          </button>
          <button onClick={() => setStep('form')} className="hover:text-blue-600 font-medium">Cancel Transaction</button>
        </div>
      </div>
    )
  }

  return (
    <div className="bg-white rounded-lg p-6 max-w-md mx-auto border-2 border-green-100 shadow-xl">
      <div className="flex items-center justify-between mb-6">
        <h3 className="text-xl font-bold text-gray-900 flex items-center space-x-2">
          <CreditCard className="w-6 h-6 text-green-600" />
          <span>Pay ₹{amount.toFixed(2)}</span>
        </h3>
        <div className="flex space-x-2 text-gray-400">
          <Shield className="w-5 h-5 text-green-500" />
          <Lock className="w-5 h-5 text-green-500" />
        </div>
      </div>

      <div className="bg-green-50 text-green-800 text-xs px-3 py-2 rounded-lg mb-6 flex items-center space-x-2">
        <CheckCircle className="w-4 h-4 text-green-600" />
        <span>End-to-End Encrypted. Compliant with PCI-DSS protocols.</span>
      </div>

      {error && (
        <div className="bg-red-50 text-red-600 text-sm p-3 rounded-lg mb-4">
          {error}
        </div>
      )}

      <div className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Card Number</label>
          <input
            type="text"
            placeholder="0000 0000 0000 0000"
            maxLength={19}
            value={cardNumber}
            onChange={(e) => {
              const val = e.target.value.replace(/\D/g, '')
              setCardNumber(val.replace(/(\d{4})/g, '$1 ').trim())
            }}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 font-mono tracking-widest"
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Expiry Date</label>
            <input
              type="text"
              placeholder="MM/YY"
              maxLength={5}
              value={expiry}
              onChange={(e) => {
                let val = e.target.value.replace(/\D/g, '')
                if (val.length > 2) val = val.substring(0, 2) + '/' + val.substring(2, 4)
                setExpiry(val)
              }}
              className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">CVV</label>
            <input
              type="password"
              placeholder="•••"
              maxLength={4}
              value={cvv}
              onChange={(e) => setCvv(e.target.value.replace(/\D/g, ''))}
              className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 font-mono"
            />
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Mobile Number (for WhatsApp OTP)</label>
          <input
            type="tel"
            placeholder="+91 98765 43210"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Name on Card</label>
          <input
            type="text"
            placeholder="JOHN DOE"
            value={name}
            onChange={(e) => setName(e.target.value.toUpperCase())}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
          />
        </div>

        <div className="flex space-x-3 mt-6">
          <button
            onClick={onCancel}
            className="flex-1 px-4 py-3 bg-gray-100 text-gray-700 font-medium rounded-lg hover:bg-gray-200"
          >
            Cancel
          </button>
          <button
            onClick={handlePay}
            disabled={isProcessing}
            className="flex-[2] px-4 py-3 bg-green-600 text-white font-bold rounded-lg hover:bg-green-700 disabled:opacity-70 flex items-center justify-center space-x-2"
          >
            {isProcessing ? (
              <span className="animate-spin border-2 border-white border-t-transparent rounded-full w-5 h-5"></span>
            ) : (
              <Lock className="w-5 h-5" />
            )}
            <span>{isProcessing ? 'Processing...' : 'Pay Securely'}</span>
          </button>
        </div>
      </div>
    </div>
  )
}
