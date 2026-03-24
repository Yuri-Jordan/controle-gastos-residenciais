import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import GerenciamentoPessoas from './components/GerenciamentoPessoas';
import GerenciamentoCategorias from './components/GerencimentoCategorias';
import GerenciamentoTransacoes from './components/GerenciamentoTransacoes';
import Relatorios from './components/Relatorios';
import './App.css';

function App() {
    return (
        <Router>
            <div className="min-h-screen bg-gray-100">
                {/* Barra de Navegação */}
                <nav className="bg-white shadow-lg">
                    <div className="max-w-7xl mx-auto px-4">
                        <div className="flex justify-between h-16">
                            <div className="flex">
                                <div className="flex-shrink-0 flex items-center">
                                    <h1 className="text-xl font-bold text-gray-800">
                                        Sistema de Controle de Despesas
                                    </h1>
                                </div>
                                <div className="hidden sm:ml-6 sm:flex sm:space-x-8">
                                    <Link
                                        to="/pessoas"
                                        className="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
                                    >
                                        Pessoas
                                    </Link>
                                    <Link
                                        to="/categorias"
                                        className="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
                                    >
                                        Categorias
                                    </Link>
                                    <Link
                                        to="/transacoes"
                                        className="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
                                    >
                                        Transações
                                    </Link>
                                    <Link
                                        to="/relatorios"
                                        className="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
                                    >
                                        Relatórios
                                    </Link>
                                </div>
                            </div>
                        </div>
                    </div>
                </nav>

                {/* Conteúdo Principal */}
                <div className="py-10">
                    <main>
                        <div className="max-w-7xl mx-auto sm:px-6 lg:px-8">
                            <Routes>
                                <Route path="/pessoas" element={<GerenciamentoPessoas />} />
                                <Route path="/categorias" element={<GerenciamentoCategorias />} />
                                <Route path="/transacoes" element={<GerenciamentoTransacoes />} />
                                <Route path="/relatorios" element={<Relatorios />} />
                                <Route path="/" element={<GerenciamentoPessoas />} />
                            </Routes>
                        </div>
                    </main>
                </div>
            </div>
        </Router>
    );
}

export default App;