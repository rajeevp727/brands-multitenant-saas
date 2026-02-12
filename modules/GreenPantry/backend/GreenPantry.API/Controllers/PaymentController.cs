using GreenPantry.Application.DTOs.Payment;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Enums;
using GreenPantry.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenPantry.API.Controllers;

[Authorize]
public class PaymentController : BaseApiController
{
    private readonly IPaymentFactoryService _paymentFactory;

    public PaymentController(IPaymentFactoryService paymentFactory)
    {
        _paymentFactory = paymentFactory;
    }

    [HttpPost("create")]
    public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] PaymentRequestDto request)
    {
        if (!await _paymentFactory.IsProviderEnabledAsync(request.Provider))
        {
            return BadRequest($"Payment provider {request.Provider} is not enabled");
        }

        var paymentService = _paymentFactory.GetPaymentService(request.Provider);
        var result = await paymentService.CreatePaymentAsync(request);
        
        return Ok(result);
    }

    [HttpPost("upi-qr")]
    public async Task<ActionResult<PaymentResponseDto>> GenerateUPIQR([FromBody] UPIQRRequestDto request)
    {
        if (!await _paymentFactory.IsProviderEnabledAsync(request.Provider))
        {
            return BadRequest($"Payment provider {request.Provider} is not enabled");
        }

        var paymentService = _paymentFactory.GetPaymentService(request.Provider);
        var result = await paymentService.GenerateUPIQRAsync(request);
        
        return Ok(result);
    }

    [HttpGet("status/{paymentId}")]
    public async Task<ActionResult<PaymentResponseDto>> GetPaymentStatus(string paymentId, [FromQuery] PaymentProvider provider)
    {
        if (!await _paymentFactory.IsProviderEnabledAsync(provider))
        {
            return BadRequest($"Payment provider {provider} is not enabled");
        }

        var paymentService = _paymentFactory.GetPaymentService(provider);
        var result = await paymentService.GetPaymentStatusAsync(paymentId);
        
        return Ok(result);
    }

    [HttpPost("refund")]
    public async Task<ActionResult<PaymentResponseDto>> ProcessRefund([FromBody] RefundRequestDto request)
    {
        if (!await _paymentFactory.IsProviderEnabledAsync(request.Provider))
        {
            return BadRequest($"Payment provider {request.Provider} is not enabled");
        }

        var paymentService = _paymentFactory.GetPaymentService(request.Provider);
        var result = await paymentService.RefundPaymentAsync(request.PaymentId, request.Amount, request.Reason);
        
        return Ok(result);
    }

    [HttpGet("providers")]
    public async Task<ActionResult<List<PaymentProvider>>> GetEnabledProviders()
    {
        var providers = await _paymentFactory.GetEnabledProvidersAsync();
        return Ok(providers);
    }

    [HttpGet("config/{provider}")]
    public async Task<ActionResult<PaymentConfiguration>> GetPaymentConfiguration(PaymentProvider provider)
    {
        var paymentService = _paymentFactory.GetPaymentService(provider);
        var config = await paymentService.GetPaymentConfigurationAsync(provider);
        
        return Ok(config);
    }
}

[ApiController]
[Route("api/[controller]/webhook")]
public class PaymentWebhookController : ControllerBase
{
    private readonly IPaymentFactoryService _paymentFactory;
    private readonly ILogger<PaymentWebhookController> _logger;

    public PaymentWebhookController(IPaymentFactoryService paymentFactory, ILogger<PaymentWebhookController> logger)
    {
        _paymentFactory = paymentFactory;
        _logger = logger;
    }

    [HttpPost("razorpay")]
    public async Task<IActionResult> RazorpayWebhook()
    {
        var signature = Request.Headers["X-Razorpay-Signature"].FirstOrDefault() ?? string.Empty;
        var payload = await new StreamReader(Request.Body).ReadToEndAsync();

        var paymentService = _paymentFactory.GetPaymentService(PaymentProvider.Razorpay);
        
        var razorpayService = paymentService as IRazorpayPaymentService;
        if (razorpayService == null || !razorpayService.VerifyRazorpayWebhookAsync(signature, payload))
        {
            _logger.LogWarning("Invalid Razorpay webhook signature");
            return BadRequest("Invalid signature");
        }

        var result = await paymentService.ProcessWebhookAsync(payload, PaymentProvider.Razorpay);
        _logger.LogInformation("Razorpay webhook processed successfully for payment {PaymentId}", result.PaymentId);
        
        return Ok();
    }

    [HttpPost("paytm")]
    public async Task<IActionResult> PaytmWebhook()
    {
        var signature = Request.Headers["X-Paytm-Signature"].FirstOrDefault() ?? string.Empty;
        var payload = await new StreamReader(Request.Body).ReadToEndAsync();

        var paymentService = _paymentFactory.GetPaymentService(PaymentProvider.Paytm);
        
        var paytmService = paymentService as IPaytmPaymentService;
        if (paytmService == null || !paytmService.VerifyPaytmWebhookAsync(signature, payload))
        {
            _logger.LogWarning("Invalid Paytm webhook signature");
            return BadRequest("Invalid signature");
        }

        var result = await paymentService.ProcessWebhookAsync(payload, PaymentProvider.Paytm);
        _logger.LogInformation("Paytm webhook processed successfully for payment {PaymentId}", result.PaymentId);
        
        return Ok();
    }

    [HttpPost("phonepe")]
    public async Task<IActionResult> PhonePeWebhook()
    {
        var signature = Request.Headers["X-Verify"].FirstOrDefault() ?? string.Empty;
        var payload = await new StreamReader(Request.Body).ReadToEndAsync();

        var paymentService = _paymentFactory.GetPaymentService(PaymentProvider.PhonePe);
        
        var phonePeService = paymentService as IPhonePePaymentService;
        if (phonePeService == null || !phonePeService.VerifyPhonePeWebhookAsync(signature, payload))
        {
            _logger.LogWarning("Invalid PhonePe webhook signature");
            return BadRequest("Invalid signature");
        }

        var result = await paymentService.ProcessWebhookAsync(payload, PaymentProvider.PhonePe);
        _logger.LogInformation("PhonePe webhook processed successfully for payment {PaymentId}", result.PaymentId);
        
        return Ok();
    }
}
