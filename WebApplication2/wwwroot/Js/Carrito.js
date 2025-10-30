// ===== FUNCIONES DEL CARRITO =====

// Alternar visibilidad del carrito
function toggleCarrito() {
    const modal = document.getElementById('modalCarrito');
    const overlay = document.getElementById('modalOverlay');

    modal.classList.toggle('abierto');
    overlay.classList.toggle('activo');
    document.body.style.overflow = modal.classList.contains('abierto') ? 'hidden' : '';

    if (modal.classList.contains('abierto')) {
        actualizarCarrito();
    }
}

// Cerrar carrito
function cerrarCarrito() {
    const modal = document.getElementById('modalCarrito');
    const overlay = document.getElementById('modalOverlay');

    modal.classList.remove('abierto');
    overlay.classList.remove('activo');
    document.body.style.overflow = '';
}

// Actualizar contador del carrito
function actualizarContadorCarrito() {
    const carrito = JSON.parse(localStorage.getItem('carrito')) || [];
    const totalItems = carrito.reduce((sum, item) => sum + item.cantidad, 0);
    document.getElementById('contadorCarrito').textContent = totalItems;
}

// Calcular precios (subtotal, IVA y total)
function calcularPrecios(carrito) {
    const subtotal = carrito.reduce((sum, item) => sum + (item.precio * item.cantidad), 0);
    const iva = subtotal * 0.16; // 16% de IVA
    const total = subtotal + iva;

    return {
        subtotal: subtotal,
        iva: iva,
        total: total
    };
}

// Actualizar vista del carrito
function actualizarCarrito() {
    const carrito = JSON.parse(localStorage.getItem('carrito')) || [];
    const carritoContent = document.getElementById('carritoContent');
    const subtotalPrecio = document.getElementById('subtotalPrecio');
    const ivaPrecio = document.getElementById('ivaPrecio');
    const totalPrecio = document.getElementById('totalPrecio');
    const btnComprar = document.getElementById('btnComprar');

    if (carrito.length === 0) {
        carritoContent.innerHTML = `
            <div class="carrito-vacio">
                <div style="font-size: 48px; margin-bottom: 10px;">🛒</div>
                <p>Tu carrito está vacío</p>
                <p style="font-size: 14px; color: #999;">Agrega algunos productos desde el catálogo</p>
            </div>
        `;
        subtotalPrecio.textContent = '$0.00 MXN';
        ivaPrecio.textContent = '$0.00 MXN';
        totalPrecio.textContent = '$0.00 MXN';
        btnComprar.disabled = true;
        return;
    }

    // Calcular precios
    const precios = calcularPrecios(carrito);

    // Mostrar productos
    carritoContent.innerHTML = carrito.map(item => {
        const subtotalItem = item.precio * item.cantidad;

        return `
            <div class="carrito-item">
                <img src="${item.imagen}" alt="${item.nombre}" class="carrito-item-img">
                <div class="carrito-item-info">
                    <div class="carrito-item-nombre">${item.nombre}</div>
                    <div class="carrito-item-precio">$${item.precio.toFixed(2)} MXN</div>
                    <div class="carrito-item-cantidad">
                        <button class="btn-cantidad" onclick="cambiarCantidad('${item.id}', -1)">-</button>
                        <span class="cantidad-numero">${item.cantidad}</span>
                        <button class="btn-cantidad" onclick="cambiarCantidad('${item.id}', 1)">+</button>
                        <span class="subtotal-item">$${subtotalItem.toFixed(2)} MXN</span>
                    </div>
                </div>
                <button class="btn-eliminar" onclick="eliminarDelCarrito('${item.id}')">🗑️</button>
            </div>
        `;
    }).join('');

    // Actualizar precios en el footer
    subtotalPrecio.textContent = `$${precios.subtotal.toFixed(2)} MXN`;
    ivaPrecio.textContent = `$${precios.iva.toFixed(2)} MXN`;
    totalPrecio.textContent = `$${precios.total.toFixed(2)} MXN`;
    btnComprar.disabled = false;
}

// Cambiar cantidad de un producto
function cambiarCantidad(productoId, cambio) {
    let carrito = JSON.parse(localStorage.getItem('carrito')) || [];
    const productoIndex = carrito.findIndex(item => item.id === productoId);

    if (productoIndex !== -1) {
        carrito[productoIndex].cantidad += cambio;

        if (carrito[productoIndex].cantidad <= 0) {
            carrito.splice(productoIndex, 1);
        }

        localStorage.setItem('carrito', JSON.stringify(carrito));
        actualizarCarrito();
        actualizarContadorCarrito();
    }
}

// Eliminar producto del carrito
function eliminarDelCarrito(productoId) {
    let carrito = JSON.parse(localStorage.getItem('carrito')) || [];
    carrito = carrito.filter(item => item.id !== productoId);
    localStorage.setItem('carrito', JSON.stringify(carrito));
    actualizarCarrito();
    actualizarContadorCarrito();
}

// Procesar compra
function procesarCompra() {
    const carrito = JSON.parse(localStorage.getItem('carrito')) || [];

    if (carrito.length === 0) {
        alert('Tu carrito está vacío');
        return;
    }

    const precios = calcularPrecios(carrito);

    const mensajeConfirmacion = `
¿Confirmar compra?

Subtotal: $${precios.subtotal.toFixed(2)} MXN
IVA (16%): $${precios.iva.toFixed(2)} MXN
Total: $${precios.total.toFixed(2)} MXN

¿Deseas continuar?
    `.trim();

    if (confirm(mensajeConfirmacion)) {
        // Aquí puedes integrar con tu sistema de pagos
        alert('¡Compra realizada con éxito! Gracias por tu compra.');
        localStorage.removeItem('carrito');
        actualizarCarrito();
        actualizarContadorCarrito();
        cerrarCarrito();
    }
}

// Inicializar carrito cuando la página carga
document.addEventListener('DOMContentLoaded', function () {
    actualizarContadorCarrito();

    // Cerrar carrito con tecla ESC
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            cerrarCarrito();
        }
    });
});