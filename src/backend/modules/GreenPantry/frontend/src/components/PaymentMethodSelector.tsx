import React, { useState, useEffect } from 'react'
import { PaymentProvider } from '../types'
import { paymentService } from '../services/paymentService'
import { CreditCard, Smartphone, QrCode, Clock } from 'lucide-react'

interface PaymentMethodSelectorProps {
  onMethodSelect: (method: string, provider?: PaymentProvider, phoneNumber?: string) => void
  selectedMethod?: string
  amount: number
}

export const PaymentMethodSelector: React.FC<PaymentMethodSelectorProps> = ({
  onMethodSelect,
  selectedMethod,
  amount
}) => {
  const [, setEnabledProviders] = useState<PaymentProvider[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [selectedApp, setSelectedApp] = useState<string>('')
  const [phoneNumber, setPhoneNumber] = useState<string>('+917032075893')

  useEffect(() => {
    loadEnabledProviders()
  }, [])

  const loadEnabledProviders = async () => {
    try {
      const providers = await paymentService.getEnabledProviders()
      setEnabledProviders(providers)
    } catch (error) {
      console.error('Failed to load enabled providers:', error)
    } finally {
      setIsLoading(false)
    }
  }

  const paymentMethods = [
    {
      id: 'upi-qr',
      name: 'UPI QR Code',
      description: 'Scan QR code with any UPI app',
      icon: <QrCode className="w-6 h-6" />,
      color: 'text-purple-600 bg-purple-50 border-purple-200',
      hoverColor: 'hover:border-purple-300 hover:bg-purple-100',
      available: true
    },
    {
      id: 'upi-app',
      name: 'UPI Apps',
      description: 'Pay with PhonePe, Google Pay, Paytm',
      icon: <Smartphone className="w-6 h-6" />,
      color: 'text-blue-600 bg-blue-50 border-blue-200',
      hoverColor: 'hover:border-blue-300 hover:bg-blue-100',
      available: true
    },
    {
      id: 'card',
      name: 'Credit/Debit Card',
      description: 'Pay with your card',
      icon: <CreditCard className="w-6 h-6" />,
      color: 'text-green-600 bg-green-50 border-green-200',
      hoverColor: 'hover:border-green-300 hover:bg-green-100',
      available: true
    },
    {
      id: 'cod',
      name: 'Cash on Delivery',
      description: 'Pay when your order arrives',
      icon: <Clock className="w-6 h-6" />,
      color: 'text-orange-600 bg-orange-50 border-orange-200',
      hoverColor: 'hover:border-orange-300 hover:bg-orange-100',
      available: true
    }
  ]

  if (isLoading) {
    return (
      <div className="space-y-4">
        <h3 className="text-lg font-semibold text-gray-900">Select Payment Method</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="animate-pulse">
              <div className="bg-gray-200 rounded-lg p-4 h-24"></div>
            </div>
          ))}
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold text-gray-900">Select Payment Method</h3>

      {/* Amount Display */}
      <div className="bg-gray-50 rounded-lg p-4">
        <div className="flex justify-between items-center">
          <span className="text-gray-600">Total Amount</span>
          <span className="text-xl font-bold text-gray-900">
            {paymentService.formatAmount(amount)}
          </span>
        </div>
      </div>

      {/* Payment Methods */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {paymentMethods.map((method) => (
          <button
            key={method.id}
            onClick={() => {
              const provider = method.id === 'upi-qr' ? PaymentProvider.PhonePe : undefined;
              onMethodSelect(method.id, provider);
            }}
            disabled={!method.available}
            className={`p-4 rounded-lg border-2 transition-all text-left ${selectedMethod === method.id
              ? `${method.color} border-current`
              : `border-gray-200 ${method.hoverColor} ${method.available ? 'text-gray-700' : 'text-gray-400 cursor-not-allowed'
              }`
              }`}
          >
            <div className="flex items-start space-x-3">
              <div className={`${method.available ? method.color.split(' ')[0] : 'text-gray-400'}`}>
                {method.icon}
              </div>
              <div className="flex-1">
                <div className="flex items-center space-x-2">
                  <h4 className="font-medium">{method.name}</h4>
                  {!method.available && (
                    <span className="text-xs bg-gray-200 text-gray-500 px-2 py-1 rounded">
                      Unavailable
                    </span>
                  )}
                </div>
                <p className="text-sm text-gray-600 mt-1">{method.description}</p>
              </div>
            </div>
          </button>
        ))}
      </div>

      {/* Provider Info for UPI */}
      {selectedMethod === 'upi-app' && (
        <div className="bg-blue-50 rounded-lg p-4">
          <h4 className="font-medium text-blue-900 mb-3">Select UPI App:</h4>
          <div className="flex flex-wrap gap-3">
            {['PhonePe', 'Google Pay', 'Paytm'].map((app) => (
              <button
                key={app}
                type="button"
                onClick={() => {
                  setSelectedApp(app);
                  let provider = PaymentProvider.Razorpay;
                  if (app === 'PhonePe') provider = PaymentProvider.PhonePe;
                  else if (app === 'Paytm') provider = PaymentProvider.Paytm;
                  onMethodSelect('upi-app', provider, phoneNumber);
                }}
                className={`inline-flex items-center space-x-2 bg-white border ${selectedApp === app ? 'border-blue-500 ring-2 ring-blue-200' : 'border-blue-200'} text-blue-900 text-sm px-4 py-2 rounded-lg font-medium cursor-pointer hover:bg-blue-50 shadow-sm transition-all`}
              >
                {app === 'PhonePe' && <img src="data:image/png;base64,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" alt="PhonePe" className="w-6 h-6 object-contain" />}
                {app === 'Google Pay' && <img src="data:image/jpeg;base64,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" alt="GPay" className="w-6 h-6 object-contain" />}
                {app === 'Paytm' && <img src="data:image/png;base64,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" alt="Paytm" className="w-8 h-6 object-contain" />}
                <span>{app}</span>
              </button>
            ))}
          </div>

          {selectedApp && (
            <div className="mt-4 bg-white p-4 rounded-lg border border-blue-100 shadow-sm animate-in fade-in slide-in-from-top-2">
              <label className="block text-sm font-medium text-blue-900 mb-2">
                Enter Phone Number linked to {selectedApp}
              </label>
              <div className="relative">
                <input
                  type="tel"
                  placeholder="+919876543210"
                  value={phoneNumber}
                  onChange={(e) => {
                    const val = e.target.value.replace(/[^\d+]/g, '');
                    setPhoneNumber(val);
                    let provider = PaymentProvider.Razorpay;
                    if (selectedApp === 'PhonePe') provider = PaymentProvider.PhonePe;
                    else if (selectedApp === 'Paytm') provider = PaymentProvider.Paytm;
                    onMethodSelect('upi-app', provider, val);
                  }}
                  className="w-full px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
                />
              </div>
            </div>
          )}
        </div>
      )}

      {/* Payment Security Info */}
      <div className="bg-green-50 rounded-lg p-4">
        <div className="flex items-start space-x-2">
          <div className="w-5 h-5 bg-green-100 rounded-full flex items-center justify-center flex-shrink-0 mt-0.5">
            <div className="w-2 h-2 bg-green-500 rounded-full"></div>
          </div>
          <div>
            <h4 className="font-medium text-green-900 text-sm">Secure Payment</h4>
            <p className="text-xs text-green-700 mt-1">
              Your payment information is encrypted and secure. We never store your card details.
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}




