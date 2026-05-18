import React, { useState, useEffect } from 'react'
import { PaymentModal } from './PaymentModal'
import { PaymentMethodSelector } from './PaymentMethodSelector'
import { CardPayment } from './CardPayment'
import { PaymentProvider, PaymentResponse, PaymentStatus } from '../types'
import { useCart } from '../hooks/useCart'
import { useAuthStore } from '../store/authStore'
import { X, MapPin } from 'lucide-react'
import { toast } from 'react-hot-toast'
import { createPortal } from 'react-dom'
import { apiService } from '../services/api'
import { useOrderStore, Order } from '../store/orderStore'

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

  const [address, setAddress] = useState<DeliveryAddress>({
    street: user?.streetAddress || '',
    city: user?.city || '',
    state: user?.state || '',
    postalCode: user?.postalCode || '',
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

  // Timer for UPI Payments
  useEffect(() => {
    let interval: NodeJS.Timeout
    if ((currentStep === 'upi_qr' || currentStep === 'upi_app_selected') && !isTimedOut) {
      interval = setInterval(() => {
        setPaymentTimer((prev) => {
          if (prev <= 1) {
            clearInterval(interval)
            setIsTimedOut(true)
            return 0
          }
          // Simulate polling status here...
          return prev - 1
        })
      }, 1000)
    }
    return () => clearInterval(interval)
  }, [currentStep, isTimedOut])

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

  const handleMethodSelect = (method: string, provider?: PaymentProvider) => {
    setSelectedMethod(method)
    setSelectedProvider(provider)
  }

  const handleProceedToPay = () => {
    setPaymentTimer(120)
    setIsTimedOut(false)
    if (selectedMethod === 'cod') {
      handleCODOrder()
    } else if (selectedMethod === 'upi-qr') {
      setCurrentStep('upi_qr')
    } else if (selectedMethod === 'upi-app') {
      setCurrentStep('upi_app_selected')
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
                {/* Order Summary (Compact) */}
                <div className="bg-gray-50 rounded-lg p-4">
                  <div className="flex justify-between items-center">
                    <span className="text-sm text-gray-600">
                      {cartState.itemCount} items
                    </span>
                    <span className="text-lg font-semibold">
                      ₹{total.toFixed(2)}
                    </span>
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
                    src={`https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(`upi://pay?pa=7032075893@ybl&pn=GreenPantry&am=${total}&cu=INR`)}`}
                    alt="UPI QR Code"
                    className={`w-48 h-48 mx-auto transition-opacity ${isTimedOut ? 'opacity-20' : 'opacity-100'}`}
                  />
                  {isTimedOut && (
                    <div className="absolute inset-0 flex items-center justify-center">
                      <span className="bg-red-100 text-red-600 font-bold px-3 py-1 rounded">QR Expired</span>
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
                <div className="flex px-6">
                  <button onClick={() => setCurrentStep('payment')} className="w-full bg-gray-100 py-3 rounded-lg text-gray-700 font-medium hover:bg-gray-200 transition-colors">
                    Cancel & Go Back
                  </button>
                </div>
              </div>
            )}

            {currentStep === 'upi_app_selected' && (
              <div className="text-center py-8">
                <div className={`animate-spin rounded-full h-12 w-12 border-b-2 mx-auto mb-4 ${isTimedOut ? 'border-red-600' : 'border-blue-600'}`}></div>
                <h3 className={`text-lg font-medium mb-2 ${isTimedOut ? 'text-red-600' : 'text-gray-900'}`}>
                  {isTimedOut ? 'Time out!' : 'Checking payment status...'}
                </h3>
                <p className="text-gray-600 mb-2">
                  Please complete the payment of ₹{total.toFixed(2)} on your mobile device.
                </p>
                {!isTimedOut ? (
                  <p className="text-sm font-medium text-orange-600 mb-6 animate-pulse">Polling payment status... ({paymentTimer}s)</p>
                ) : (
                  <p className="text-sm font-medium text-red-600 mb-6">Request expired. Please try again.</p>
                )}
                <div className="flex px-6">
                  <button onClick={() => setCurrentStep('payment')} className="w-full bg-gray-100 py-3 rounded-lg text-gray-700 font-medium hover:bg-gray-200 transition-colors">
                    Cancel & Go Back
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
