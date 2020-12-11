using System;
using System.Windows;
using System.Windows.Media;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace SharpDXLib
{
    /// <summary>
    /// Create SharpDx swapchain hosted in the controls parent Hwnd
    /// Resources created on Loaded, destroyed on Unloaded. 
    /// </summary>
    public abstract class DirectXComponent : Win32Control
    {
        private Device _device;

        // цепочка обмена буффером
        private SwapChain _swapChain;

        // задний буффер
        private Texture2D _backBuffer;

        //объект представления данных
        private RenderTargetView _renderTargetView;
        protected Texture2D BackBuffer => _backBuffer;
        //protected RenderTargetView RenderTargetView => _renderTargetView;

        //длина
        protected int SurfaceWidth { get; private set; }

        //ширена
        protected int SurfaceHeight { get; private set; }



        protected DirectXComponent()
        {
        }

        //метод в котором происходит отрисовка
        protected override sealed void Initialize()
        {
            InternalInitialize();

            //поверхность отображения на котором происходит отрисовка
            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        protected override sealed void Uninitialize()
        {

            CompositionTarget.Rendering -= OnCompositionTargetRendering;

            InternalUninitialize();
        }

        protected sealed override void Resized()
        {
            InternalUninitialize();
            InternalInitialize();
        }

        private void OnCompositionTargetRendering(object sender, EventArgs eventArgs)
        {

            BeginRender();
            Render();
            EndRender();

        }

        private double GetDpiScale()
        {
            PresentationSource source = PresentationSource.FromVisual(this);

            return source.CompositionTarget.TransformToDevice.M11;
        }

        /// <summary>
        /// Create required DirectX resources.
        /// Derived calls should begin with base.InternalInitialize()
        /// </summary>
        protected virtual void InternalInitialize()
        {
            var dpiScale = GetDpiScale();

            //длина  и ширина окна, которая будет подстраиваться в зависимости от изменения размера окна
            SurfaceWidth = (int)(ActualWidth < 0 ? 0 : Math.Ceiling(ActualWidth * dpiScale));
            SurfaceHeight = (int)(ActualHeight < 0 ? 0 : Math.Ceiling(ActualHeight * dpiScale));

            //цепочка обмена для буфферов
            var swapChainDescription = new SwapChainDescription
            {
                //окно, в котором будет происходить отображение
                OutputHandle = Hwnd,

                //кол-во буферов
                BufferCount = 1,


                //оконный или полноэкранный режим
                IsWindowed = true,

                //задний буфер(размер окна, частота обновлений в снкунду, формат буфера)
                ModeDescription = new ModeDescription(SurfaceWidth, SurfaceHeight, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                //для сглаживания отрисованных фигур
                SampleDescription = new SampleDescription(1, 0),

                // процесс получения процессором заднего буфера
                SwapEffect = SwapEffect.Discard,

                //использует элемент отображения как RenderTargetOutput 
                Usage = Usage.RenderTargetOutput
            };

            //как устройсво будет обмениваться с цепочкой(он будет использавоть граф. процессор, флаг с поддержкой ? , дескриптор цепочки обмена, устройство , цепочка обмена   )
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDescription, out _device, out _swapChain);

            // New RenderTargetView from the backbuffer
            _backBuffer = Resource.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderTargetView = new RenderTargetView(_device, _backBuffer);
        }

        /// <summary>
        /// Destory all DirectX resources. Очистка
        /// Derived methods should end with base.InternalUninitialize();
        /// </summary>
        protected virtual void InternalUninitialize()
        {
            Utilities.Dispose(ref _renderTargetView);
            Utilities.Dispose(ref _backBuffer);
            Utilities.Dispose(ref _swapChain);
            Utilities.Dispose(ref _device);

        }

        /// <summary>
        /// Begin render.
        /// Derived methods should begin with base.BeginRender()
        /// </summary>
        protected virtual void BeginRender()
        {

            //_device.ImmediateContext.Rasterizer.SetViewport(0, 0, (float)ActualWidth, (float)ActualHeight);

            //устанавливает объект рендеринга на тот, что был создан ранее
            _device.ImmediateContext.OutputMerger.SetRenderTargets(_renderTargetView);
        }

        /// <summary>
        /// Finish render.
        /// Derived methods must call base.EndRender() 
        /// </summary>
        protected virtual void EndRender()
        {
            //заменяет заднюю часть передним буффером (1 - вертикальная синхронизация ) 
            _swapChain.Present(1, PresentFlags.None);
        }

        ///!!!!!!!!!!!!!! сюда будет вставляться отрисовка в каждом кадре!!!!!!!!!!
		protected abstract void Render();
    }
}
