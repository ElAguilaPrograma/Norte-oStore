 document.getElementById('loginForm').addEventListener('submit', function(e) {
            e.preventDefault();
            
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;
            const message = document.getElementById('message');
            
            // Credenciales de ejemplo (en un caso real, esto vendría de una base de datos)
            const validUsers = {
                'admin': '123456',
                'usuario': 'password',
                'test': 'test123'
            };
            
            // Validar credenciales
            if (validUsers[username] && validUsers[username] === password) {
                showMessage('¡Inicio de sesión exitoso! Redirigiendo...', 'success');
                
                // Simular redirección después de 2 segundos
                setTimeout(() => {
                    alert(`¡Bienvenido, ${username}!`);
                    // Limpiar formulario
                    document.getElementById('loginForm').reset();
                }, 2000);
            } else {
                showMessage('Usuario o contraseña incorrectos', 'error');
            }
        });

        // Redirigir al registro (en una aplicación real, esto llevaría a otra página)
        document.getElementById('registerLink').addEventListener('click', function(e) {
            e.preventDefault();
            showMessage('Redirigiendo al formulario de registro...', 'success');
            
            // En una aplicación real, aquí redirigirías a la página de registro
            // window.location.href = 'registro.html';
            
            // Simulación de redirección
            setTimeout(() => {
                alert('Aquí irías a la página de registro');
            }, 1000);
        });

        function showMessage(text, type) {
            const message = document.getElementById('message');
            message.textContent = text;
            message.className = `message ${type}`;
            message.style.display = 'block';
            
            // Ocultar mensaje después de 5 segundos
            setTimeout(() => {
                message.style.display = 'none';
            }, 5000);
        }

        // Efecto adicional: Limpiar mensaje al empezar a escribir
        document.getElementById('username').addEventListener('input', hideMessage);
        document.getElementById('password').addEventListener('input', hideMessage);

        function hideMessage() {
            document.getElementById('message').style.display = 'none';
        }