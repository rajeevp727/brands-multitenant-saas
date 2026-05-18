import React, { useState, useEffect } from 'react'
import { PaymentModal } from './PaymentModal'
import { PaymentMethodSelector } from './PaymentMethodSelector'
import { CardPayment } from './CardPayment'
import { PaymentProvider, PaymentResponse, PaymentStatus } from '../types'
import { useCart } from '../hooks/useCart'
import { useAuthStore } from '../store/authStore'
import { X, MapPin, RefreshCw, Smartphone, Check, Loader2 } from 'lucide-react'
import { toast } from 'react-hot-toast'
import { createPortal } from 'react-dom'
import { apiService } from '../services/api'
import { useOrderStore, Order } from '../store/orderStore'
import { paymentService } from '../services/paymentService'

interface CheckoutModalProps {
  isOpen: boolean
  onClose: () => void
  onOrderSuccess?: (orderId: string) => void
}

interface DeliveryAddress {
  street: string
  city: string
  state: string
  postalCode: string
  country: string
  latitude: number
  longitude: number
}

export const CheckoutModal: React.FC<CheckoutModalProps> = ({
  isOpen,
  onClose,
  onOrderSuccess
}) => {
  const { state: cartState, clearCart } = useCart()
  const { user } = useAuthStore()
  const addOrder = useOrderStore((state) => state.addOrder)
  const simulateProgress = useOrderStore((state) => state.simulateProgress)
  const [currentStep, setCurrentStep] = useState<'address' | 'payment' | 'processing' | 'card_form' | 'upi_qr' | 'upi_app_selected'>('address')
  const [selectedMethod, setSelectedMethod] = useState<string>('')
  const [selectedProvider, setSelectedProvider] = useState<PaymentProvider | undefined>()
  const [, setIsProcessing] = useState(false)
  const [orderId, setOrderId] = useState<string>('')
  const [paymentTimer, setPaymentTimer] = useState(120)
  const [isTimedOut, setIsTimedOut] = useState(false)
  const [paymentResponse, setPaymentResponse] = useState<PaymentResponse | null>(null)
  const [isLoadingPayment, setIsLoadingPayment] = useState(false)
  const [upiPhoneNumber, setUpiPhoneNumber] = useState<string>('7032075893')

  const initiateUPIPayment = async (providerOverride?: PaymentProvider) => {
    setIsLoadingPayment(true)
    const provider = providerOverride || selectedProvider || PaymentProvider.PhonePe
    try {
      const request = {
        orderId,
        amount: total,
        currency: 'INR',
        provider,
        customerName: user?.name || 'Customer',
        customerEmail: user?.email || 'customer@test.com',
        customerPhone: upiPhoneNumber || user?.phoneNumber || '+917032075893',
        description: `Checkout for Order #${orderId}`,
        expiryMinutes: 5
      }

      console.log('Initiating UPI payment:', request)
      const response = await paymentService.generateUPIQR(request)
      setPaymentResponse(response)
      setPaymentTimer(120)
      setIsTimedOut(false)

      // If mobile app is selected, open deep link
      if (currentStep === 'upi_app_selected' || selectedMethod === 'upi-app') {
        const upiLink = response.upiQRData || `upi://pay?pa=7032075893@ybl&pn=GreenPantry&am=${total}&cu=INR`;
        window.location.href = upiLink;
      }

      toast.success(`Payment request initiated with ${provider}`)
    } catch (error: any) {
      console.error('Failed to initiate UPI payment:', error)
      toast.error(error.response?.data?.message || 'Failed to initiate payment')
    } finally {
      setIsLoadingPayment(false)
    }
  }

  const [address, setAddress] = useState<DeliveryAddress>({
    street: user?.streetAddress || 'Hyderabad',
    city: user?.city || 'Hyderabad',
    state: user?.state || 'Hyderabad',
    postalCode: user?.postalCode || 'Hyderabad',
    country: user?.country || 'India',
    latitude: user?.latitude || 0,
    longitude: user?.longitude || 0
  })

  // Calculate totals
  const subtotal = cartState.total
  const deliveryFee = subtotal > 500 ? 0 : 50 // Free delivery over ₹500
  const tax = subtotal * 0.18 // 18% GST
  const total = subtotal + deliveryFee + tax

  // Generate order number
  useEffect(() => {
    if (isOpen) {
      setOrderId(`GP${Date.now()}`)
    }
  }, [isOpen])

  // Timer and Polling for UPI Payments
  useEffect(() => {
    let interval: NodeJS.Timeout
    let pollInterval: NodeJS.Timeout

    if ((currentStep === 'upi_qr' || currentStep === 'upi_app_selected') && !isTimedOut) {
      // Countdown Timer
      interval = setInterval(() => {
        setPaymentTimer((prev) => {
          if (prev <= 1) {
            clearInterval(interval)
            setIsTimedOut(true)
            return 0
          }
          return prev - 1
        })
      }, 1000)

      // Polling backend status every 4 seconds
      pollInterval = setInterval(async () => {
        if (paymentResponse?.paymentId) {
          try {
            const provider = paymentResponse.provider || selectedProvider || PaymentProvider.PhonePe
            const response = await paymentService.getPaymentStatus(paymentResponse.paymentId, provider)

            if (response.status === 'Success') {
              clearInterval(interval)
              clearInterval(pollInterval)
              handlePaymentSuccess(response)
            } else if (response.status === 'Failed') {
              clearInterval(interval)
              clearInterval(pollInterval)
              toast.error('Payment failed')
              setCurrentStep('payment')
            }
          } catch (error) {
            console.error('Error polling payment status:', error)
          }
        }
      }, 4000)
    }

    return () => {
      clearInterval(interval)
      clearInterval(pollInterval)
    }
  }, [currentStep, isTimedOut, paymentResponse])

  const handleAddressSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!address.street || !address.city || !address.state || !address.postalCode) {
      toast.error('Please fill in all required address fields')
      return
    }

    // Save address to user profile if logged in
    if (user && user.id) {
      try {
        await apiService.updateAddress(address)
        // Update local user state
        const { setUser } = useAuthStore.getState()
        setUser({
          ...user,
          streetAddress: address.street,
          city: address.city,
          state: address.state,
          postalCode: address.postalCode,
          country: address.country,
          latitude: address.latitude,
          longitude: address.longitude
        })
      } catch (error) {
        console.error('Failed to save address to profile', error)
      }
    }

    setCurrentStep('payment')
  }

  const handleMethodSelect = (method: string, provider?: PaymentProvider, phoneNumber?: string) => {
    setSelectedMethod(method)
    setSelectedProvider(provider)
    if (phoneNumber) {
      setUpiPhoneNumber(phoneNumber)
    }
  }

  const handleProceedToPay = () => {
    if (selectedMethod === 'cod') {
      handleCODOrder()
    } else if (selectedMethod === 'upi-qr') {
      setCurrentStep('upi_qr')
      initiateUPIPayment()
    } else if (selectedMethod === 'upi-app') {
      setCurrentStep('upi_app_selected')
      initiateUPIPayment()
    } else if (selectedMethod === 'card') {
      setCurrentStep('card_form')
    } else {
      toast.error('Please select a payment method')
    }
  }

  const handleCODOrder = async () => {
    setIsProcessing(true)
    try {
      // Create order for COD
      const newOrder: Order = {
        id: orderId,
        restaurant: {
          name: 'GreenPantry Selected Kitchen',
          image: 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=100',
          rating: 4.5
        },
        items: cartState.items.map(item => ({
          name: `${item.quantity}x ${item.menuItem.name} ${item.variant !== 'Regular' ? `(${item.variant})` : ''}`,
          quantity: item.quantity,
          price: item.menuItem.price
        })),
        total,
        status: 'Pending',
        orderDate: new Date().toISOString(),
        deliveryDate: null,
        deliveryAddress: `${address.street}, ${address.city}, ${address.postalCode}`,
        paymentMethod: 'Cash On Delivery',
        deliveryFee,
        discount: 0
      }

      console.log('Creating COD order:', newOrder)

      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 2000))

      addOrder(newOrder)
      simulateProgress(orderId)

      toast.success('Order placed successfully! You will pay on delivery.')
      clearCart()
      onOrderSuccess?.(orderId)
      onClose()
    } catch (error) {
      toast.error('Failed to place order. Please try again.')
    } finally {
      setIsProcessing(false)
    }
  }

  const handlePaymentSuccess = (payment: PaymentResponse) => {
    setIsProcessing(true)

    // Create order with payment details
    const newOrder: Order = {
      id: orderId,
      restaurant: {
        name: 'GreenPantry Selected Kitchen',
        image: 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=100',
        rating: 4.8
      },
      items: cartState.items.map(item => ({
        name: `${item.quantity}x ${item.menuItem.name} ${item.variant !== 'Regular' ? `(${item.variant})` : ''}`,
        quantity: item.quantity,
        price: item.menuItem.price
      })),
      total,
      status: 'Pending',
      orderDate: new Date().toISOString(),
      deliveryDate: null,
      deliveryAddress: `${address.street}, ${address.city}, ${address.postalCode}`,
      paymentMethod: selectedMethod === 'upi-qr' || selectedMethod === 'upi-app' ? 'UPI' : 'Card',
      deliveryFee,
      discount: 0
    }

    console.log('Creating paid order:', newOrder)

    addOrder(newOrder)
    simulateProgress(orderId)

    toast.success('Payment successful! Order placed.')
    clearCart()
    onOrderSuccess?.(orderId)
    onClose()
  }

  const handlePaymentFailed = (error: string) => {
    toast.error(`Payment failed: ${error}`)
  }

  // Attempt to trigger UPI App Intent Deep Link
  useEffect(() => {
    if (currentStep === 'upi_app_selected') {
      const deepLink = `upi://pay?pa=7032075893@ybl&pn=GreenPantry&am=${total}&cu=INR`;
      window.location.href = deepLink;
    }
  }, [currentStep, total]);

  if (!isOpen) return null

  return createPortal(
    <div className="fixed inset-0 z-[110] overflow-y-auto">
      <div className="flex min-h-screen items-center justify-center p-4">
        {/* Backdrop */}
        <div
          className="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
          onClick={onClose}
        />

        {/* Modal Content */}
        <div className="relative bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
          {/* Header */}
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <div className="flex items-center space-x-3">
              <h2 className="text-xl font-semibold text-gray-900">
                Checkout
              </h2>
              <span className="text-sm text-gray-500">
                Order #{orderId}
              </span>
            </div>
            <button
              onClick={onClose}
              className="p-1 hover:bg-gray-100 rounded-full transition-colors"
            >
              <X className="w-5 h-5 text-gray-600" />
            </button>
          </div>

          {/* Content */}
          <div className="p-6">
            {currentStep === 'address' && (
              <div className="space-y-6">
                {/* Order Summary */}
                <div className="bg-gray-50 rounded-lg p-4">
                  <h3 className="font-medium text-gray-900 mb-3">Order Summary</h3>
                  <div className="space-y-2">
                    {cartState.items.map((item) => (
                      <div key={`${item.menuItem.id}-${item.variant}`} className="flex justify-between text-sm">
                        <span className="text-gray-600">
                          {item.menuItem.name} {item.variant !== 'Regular' ? `(${item.variant})` : ''} x {item.quantity}
                        </span>
                        <span className="font-medium">
                          ₹{((item.menuItem.price + (item.menuItem.variants?.find(v => v.name === item.variant)?.priceModifier || 0)) * item.quantity).toFixed(2)}
                        </span>
                      </div>
                    ))}
                    <div className="border-t pt-2 mt-2">
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-600">Subtotal</span>
                        <span>₹{subtotal.toFixed(2)}</span>
                      </div>
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-600">Delivery Fee</span>
                        <span>{deliveryFee === 0 ? 'Free' : `₹${deliveryFee.toFixed(2)}`}</span>
                      </div>
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-600">Tax (18%)</span>
                        <span>₹{tax.toFixed(2)}</span>
                      </div>
                      <div className="flex justify-between font-semibold text-lg border-t pt-2 mt-2">
                        <span>Total</span>
                        <span>₹{total.toFixed(2)}</span>
                      </div>
                    </div>
                  </div>
                </div>

                {/* Delivery Address Form */}
                <form onSubmit={handleAddressSubmit} className="space-y-4">
                  <h3 className="font-medium text-gray-900 flex items-center space-x-2">
                    <MapPin className="w-5 h-5" />
                    <span>Delivery Address</span>
                  </h3>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Street Address *
                      </label>
                      <input
                        type="text"
                        value={address.street}
                        onChange={(e) => setAddress({ ...address, street: e.target.value })}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        placeholder="Enter street address"
                        required
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        City *
                      </label>
                      <input
                        type="text"
                        value={address.city}
                        onChange={(e) => setAddress({ ...address, city: e.target.value })}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        placeholder="Enter city"
                        required
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        State *
                      </label>
                      <input
                        type="text"
                        value={address.state}
                        onChange={(e) => setAddress({ ...address, state: e.target.value })}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        placeholder="Enter state"
                        required
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Postal Code *
                      </label>
                      <input
                        type="text"
                        value={address.postalCode}
                        onChange={(e) => setAddress({ ...address, postalCode: e.target.value })}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        placeholder="Enter postal code"
                        required
                      />
                    </div>
                  </div>

                  <div className="flex justify-end space-x-3">
                    <button
                      type="button"
                      onClick={onClose}
                      className="px-4 py-2 text-gray-600 hover:text-gray-800 transition-colors"
                    >
                      Cancel
                    </button>
                    <button
                      type="submit"
                      className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                    >
                      Continue to Payment
                    </button>
                  </div>
                </form>
              </div>
            )}

            {currentStep === 'payment' && (
              <div className="space-y-6">
                {/* Order Summary (Detailed Bill Breakup) */}
                <div className="bg-gradient-to-br from-blue-50 to-white rounded-xl p-5 border border-blue-100 shadow-sm space-y-3">
                  <h4 className="text-sm font-bold text-blue-900 uppercase tracking-wider mb-2">Bill Breakup</h4>
                  
                  {/* Cart items summary */}
                  <div className="space-y-2 border-b border-blue-50 pb-3 max-h-32 overflow-y-auto pr-1">
                    {cartState.items.map((item) => (
                      <div key={`${item.menuItem.id}-${item.variant}`} className="flex justify-between items-center text-sm text-gray-700">
                        <span className="truncate max-w-[200px]">
                          {item.quantity}x {item.menuItem.name} 
                          {item.variant !== 'Regular' && <span className="text-xs text-gray-500 ml-1">({item.variant})</span>}
                        </span>
                        <span className="font-medium">₹{(item.menuItem.price * item.quantity).toFixed(2)}</span>
                      </div>
                    ))}
                  </div>

                  <div className="space-y-2 text-sm text-gray-600">
                    <div className="flex justify-between items-center">
                      <span>Items Subtotal</span>
                      <span className="font-semibold text-gray-900">₹{subtotal.toFixed(2)}</span>
                    </div>
                    <div className="flex justify-between items-center">
                      <span>Delivery Fee</span>
                      {deliveryFee === 0 ? (
                        <span className="text-green-600 font-bold">FREE</span>
                      ) : (
                        <span className="font-semibold text-gray-900">₹{deliveryFee.toFixed(2)}</span>
                      )}
                    </div>
                    <div className="flex justify-between items-center">
                      <span>GST (18%)</span>
                      <span className="font-semibold text-gray-900">₹{tax.toFixed(2)}</span>
                    </div>
                  </div>

                  <div className="flex justify-between items-center border-t border-blue-100 pt-3 mt-1">
                    <span className="text-base font-bold text-gray-900">To Pay</span>
                    <span className="text-xl font-extrabold text-blue-600">₹{total.toFixed(2)}</span>
                  </div>
                </div>

                {/* Payment Method Selection */}
                <PaymentMethodSelector
                  onMethodSelect={handleMethodSelect}
                  selectedMethod={selectedMethod}
                  amount={total}
                />

                {/* Payment Processing button */}
                {selectedMethod && (
                  <button
                    onClick={handleProceedToPay}
                    className="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-bold hover:bg-blue-700 transition-colors flex items-center justify-center space-x-2 shadow-lg"
                  >
                    <span>Proceed to Pay</span>
                    <span>₹{total.toFixed(2)}</span>
                  </button>
                )}
              </div>
            )}

            {currentStep === 'card_form' && (
              <CardPayment
                amount={total}
                onSuccess={() => {
                  handlePaymentSuccess({
                    paymentId: `card_${Date.now()}`,
                    orderId,
                    provider: PaymentProvider.Razorpay,
                    status: PaymentStatus.Success,
                    amount: total,
                    currency: 'INR',
                    providerTransactionId: `txn_${Date.now()}`
                  })
                }}
                onCancel={() => setCurrentStep('payment')}
              />
            )}

            {currentStep === 'upi_qr' && (
              <div className="text-center py-6 bg-white rounded-xl shadow-sm border border-purple-100 max-w-sm mx-auto">
                <h3 className="text-lg font-bold text-purple-900 mb-4">Scan to Pay ₹{total.toFixed(2)}</h3>
                <div className="bg-white p-4 rounded-lg border-2 border-purple-200 inline-block mb-6 relative">
                  <img
                    src={
                      paymentResponse?.upiQRCode
                        ? (paymentResponse.upiQRCode.startsWith('data:')
                          ? paymentResponse.upiQRCode
                          : `data:image/png;base64,${paymentResponse.upiQRCode}`)
                        : `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(paymentResponse?.upiQRData || `upi://pay?pa=7032075893@ybl&pn=GreenPantry&am=${total}&cu=INR`)}`
                    }
                    alt="UPI QR Code"
                    className={`w-48 h-48 mx-auto transition-opacity ${isTimedOut || isLoadingPayment ? 'opacity-20' : 'opacity-100'}`}
                  />
                  {(isTimedOut || isLoadingPayment) && (
                    <div className="absolute inset-0 flex items-center justify-center">
                      <span className="bg-red-100 text-red-600 font-bold px-3 py-1 rounded">
                        {isLoadingPayment ? 'Generating...' : 'QR Expired'}
                      </span>
                    </div>
                  )}
                </div>
                {!isTimedOut ? (
                  <p className={`text-sm font-medium mb-4 animate-pulse ${paymentTimer > 60 ? 'text-green-600' : paymentTimer > 30 ? 'text-orange-600' : 'text-red-600'}`}>
                    Checking payment status... ({paymentTimer}s)
                  </p>
                ) : (
                  <p className="text-sm font-medium text-red-600 mb-4">Time out! Please try again.</p>
                )}
                {isTimedOut ? (
                  <div className="flex gap-4 px-6">
                    <button
                      onClick={() => initiateUPIPayment()}
                      disabled={isLoadingPayment}
                      className="flex-1 bg-purple-600 text-white py-3 rounded-lg font-bold hover:bg-purple-700 shadow-md hover:shadow-lg transition-all transform hover:-translate-y-0.5 active:translate-y-0 flex items-center justify-center space-x-2 disabled:opacity-50"
                    >
                      <RefreshCw className={`w-4 h-4 ${isLoadingPayment ? 'animate-spin' : ''}`} />
                      <span>{isLoadingPayment ? 'Retrying...' : 'Retry'}</span>
                    </button>
                    <button
                      onClick={() => setCurrentStep('payment')}
                      className="flex-1 bg-gray-100 text-gray-700 py-3 rounded-lg font-medium hover:bg-gray-200 transition-colors border border-gray-200"
                    >
                      Go Back
                    </button>
                  </div>
                ) : (
                  <div className="flex px-6">
                    <button onClick={() => setCurrentStep('payment')} className="w-full bg-gray-100 py-3 rounded-lg text-gray-700 font-medium hover:bg-gray-200 transition-colors">
                      Cancel & Go Back
                    </button>
                  </div>
                )}
              </div>
            )}

            {currentStep === 'upi_app_selected' && (
              <div className="text-center py-6 max-w-md mx-auto space-y-6">
                <div className="bg-gradient-to-br from-purple-50 to-blue-50 border border-blue-100 rounded-xl p-5 shadow-sm text-left relative overflow-hidden">
                  <div className="absolute top-0 right-0 w-24 h-24 bg-blue-100 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob"></div>
                  <div className="absolute bottom-0 left-0 w-24 h-24 bg-purple-100 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-2000"></div>

                  <div className="flex items-center space-x-2 text-blue-800 font-bold mb-3">
                    <Smartphone className="w-5 h-5" />
                    <span className="text-sm uppercase tracking-wider">UPI Notification simulator</span>
                  </div>

                  <h4 className="text-base font-bold text-gray-900 mb-2">Why didn't you receive a notification?</h4>
                  <p className="text-xs text-gray-600 leading-relaxed mb-4">
                    Since the system is running in <strong>Sandbox Mode</strong> locally, physical push notifications cannot be delivered to your physical phone's UPI app. Instead, you can fully test and experience the payment flow using the interactive controls below!
                  </p>

                  {/* Simulated push notification */}
                  <div className="bg-white border border-gray-100 rounded-xl p-4 shadow-sm hover:shadow-md transition-shadow relative z-10 space-y-3">
                    <div className="flex items-center justify-between pb-2 border-b border-gray-50">
                      <div className="flex items-center space-x-2">
                        <div className="w-8 h-8 rounded-lg bg-purple-600 flex items-center justify-center text-white font-black text-xs shadow-sm">
                          Pe
                        </div>
                        <div>
                          <span className="block text-xs font-semibold text-gray-900">PhonePe</span>
                          <span className="block text-[10px] text-gray-400">GreenPantry Payment Request</span>
                        </div>
                      </div>
                      <span className="text-[10px] text-gray-400 font-medium">Just now</span>
                    </div>

                    <p className="text-xs text-gray-700">
                      <strong>GreenPantry</strong> is requesting a payment of <strong className="text-purple-600 font-bold text-sm">₹{total.toFixed(2)}</strong>.
                    </p>

                    <div className="flex gap-2 pt-1">
                      <button
                        onClick={() => {
                          if (paymentResponse) {
                            handlePaymentSuccess({
                              ...paymentResponse,
                              status: PaymentStatus.Success
                            });
                          }
                        }}
                        className="flex-1 bg-purple-600 hover:bg-purple-700 text-white py-2 rounded-lg text-xs font-bold shadow-sm hover:shadow transition-all transform hover:-translate-y-0.5 active:translate-y-0 flex items-center justify-center space-x-1"
                      >
                        <Check className="w-3.5 h-3.5" />
                        <span>Approve Pay</span>
                      </button>
                      <button
                        onClick={() => {
                          toast.error('Payment declined by user');
                          setCurrentStep('payment');
                        }}
                        className="flex-1 bg-red-50 hover:bg-red-100 text-red-700 py-2 rounded-lg text-xs font-bold transition-all flex items-center justify-center space-x-1"
                      >
                        <X className="w-3.5 h-3.5" />
                        <span>Decline</span>
                      </button>
                    </div>
                  </div>
                </div>

                <div className="bg-white border border-gray-100 rounded-xl p-4 text-left space-y-2">
                  <div className="flex items-center justify-between text-xs text-gray-500">
                    <span>Selected App:</span>
                    <span className="font-semibold text-gray-900">{selectedProvider || 'PhonePe'}</span>
                  </div>
                  <div className="flex items-center justify-between text-xs text-gray-500">
                    <span>Phone Number:</span>
                    <span className="font-semibold text-gray-900">{upiPhoneNumber}</span>
                  </div>
                  <div className="flex items-center justify-between text-xs text-gray-500">
                    <span>Status:</span>
                    <span className="inline-flex items-center space-x-1 text-orange-600 font-bold animate-pulse">
                      <Loader2 className="w-3 h-3 animate-spin" />
                      <span>Waiting ({paymentTimer}s)</span>
                    </span>
                  </div>
                </div>

                <div className="flex gap-4">
                  <button
                    onClick={() => initiateUPIPayment()}
                    disabled={isLoadingPayment}
                    className="flex-1 bg-blue-600 hover:bg-blue-700 text-white py-3 rounded-lg font-bold shadow-md hover:shadow-lg transition-all transform hover:-translate-y-0.5 active:translate-y-0 flex items-center justify-center space-x-2 disabled:opacity-50"
                  >
                    <RefreshCw className={`w-4 h-4 ${isLoadingPayment ? 'animate-spin' : ''}`} />
                    <span>{isLoadingPayment ? 'Resending...' : 'Resend Request'}</span>
                  </button>
                  <button
                    onClick={() => setCurrentStep('payment')}
                    className="flex-1 bg-gray-100 text-gray-700 py-3 rounded-lg font-medium hover:bg-gray-200 transition-colors border border-gray-200"
                  >
                    Go Back
                  </button>
                </div>
              </div>
            )}

            {currentStep === 'processing' && (
              <div className="text-center py-8">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
                <h3 className="text-lg font-medium text-gray-900 mb-2">
                  Processing your order...
                </h3>
                <p className="text-gray-600">
                  Please wait while we process your payment and create your order.
                </p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>,
    document.body
  )
}
