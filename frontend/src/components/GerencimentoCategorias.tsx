import React, { useState, useEffect } from 'react';
import { Categoria, CriarCategoria, FinalidadeCategoria } from '../types';
import api from '../services/api';

const GerenciamentoCategorias: React.FC = () => {
    const [categorias, setCategorias] = useState<Categoria[]>([]);
    const [dadosFormulario, setDadosFormulario] = useState<CriarCategoria>({
        descricao: '',
        finalidade: FinalidadeCategoria.Ambos
    });
    const [erro, setErro] = useState<string>('');
    const [sucesso, setSucesso] = useState<string>('');

    useEffect(() => {
        carregarCategorias();
    }, []);

    const carregarCategorias = async () => {
        try {
            const dados = await api.obterCategorias();
            setCategorias(dados);
        } catch (err) {
            setErro('Falha ao carregar categorias');
            console.error(err);
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setDadosFormulario({
            ...dadosFormulario,
            [name]: name === 'finalidade' ? parseInt(value) : value
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setErro('');
        setSucesso('');

        // Validação
        if (!dadosFormulario.descricao.trim()) {
            setErro('Descrição é obrigatória');
            return;
        }
        if (dadosFormulario.descricao.length > 400) {
            setErro('Descrição não pode exceder 400 caracteres');
            return;
        }

        try {
            await api.criarCategoria(dadosFormulario);
            setSucesso('Categoria criada com sucesso');
            setDadosFormulario({ descricao: '', finalidade: FinalidadeCategoria.Ambos });
            await carregarCategorias();
            
            // Limpar mensagem de sucesso após 3 segundos
            setTimeout(() => setSucesso(''), 3000);
        } catch (err: any) {
            setErro(err.response?.data || 'Falha ao criar categoria');
            console.error(err);
        }
    };

    const obterTextoFinalidade = (finalidade: FinalidadeCategoria): string => {
        switch (finalidade) {
            case FinalidadeCategoria.Despesa:
                return 'Despesa';
            case FinalidadeCategoria.Receita:
                return 'Receita';
            case FinalidadeCategoria.Ambos:
                return 'Ambos';
            default:
                return 'Desconhecido';
        }
    };

    const obterCorFinalidade = (finalidade: FinalidadeCategoria): string => {
        switch (finalidade) {
            case FinalidadeCategoria.Despesa:
                return 'bg-red-100 text-red-800';
            case FinalidadeCategoria.Receita:
                return 'bg-green-100 text-green-800';
            case FinalidadeCategoria.Ambos:
                return 'bg-blue-100 text-blue-800';
            default:
                return 'bg-gray-100 text-gray-800';
        }
    };

    return (
        <div className="bg-white shadow rounded-lg p-6">
            <h2 className="text-2xl font-bold mb-6">Gerenciamento de Categorias</h2>
            
            {/* Formulário para criar categorias */}
            <form onSubmit={handleSubmit} className="mb-8 p-4 bg-gray-50 rounded-lg">
                <h3 className="text-lg font-semibold mb-4">Criar Nova Categoria</h3>
                
                {erro && (
                    <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">
                        {erro}
                    </div>
                )}
                
                {sucesso && (
                    <div className="mb-4 p-3 bg-green-100 text-green-700 rounded">
                        {sucesso}
                    </div>
                )}
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Descrição *
                        </label>
                        <input
                            type="text"
                            name="descricao"
                            value={dadosFormulario.descricao}
                            onChange={handleInputChange}
                            maxLength={400}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                        />
                        <p className="text-xs text-gray-500 mt-1">
                            {dadosFormulario.descricao.length}/400 caracteres
                        </p>
                    </div>
                    
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Finalidade *
                        </label>
                        <select
                            name="finalidade"
                            value={dadosFormulario.finalidade}
                            onChange={handleInputChange}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                        >
                            <option value={FinalidadeCategoria.Despesa}>Apenas Despesa</option>
                            <option value={FinalidadeCategoria.Receita}>Apenas Receita</option>
                            <option value={FinalidadeCategoria.Ambos}>Ambos</option>
                        </select>
                    </div>
                </div>
                
                <div className="mt-4">
                    <button
                        type="submit"
                        className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                        Criar Categoria
                    </button>
                </div>
            </form>
            
            {/* Lista de categorias (somente leitura) */}
            <div>
                <h3 className="text-lg font-semibold mb-4">Lista de Categorias</h3>
                {categorias.length === 0 ? (
                    <p className="text-gray-500">Nenhuma categoria encontrada.</p>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Descrição
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Finalidade
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {categorias.map((categoria) => (
                                    <tr key={categoria.id}>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            {categoria.descricao}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${obterCorFinalidade(categoria.finalidade)}`}>
                                                {obterTextoFinalidade(categoria.finalidade)}
                                            </span>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
};

export default GerenciamentoCategorias;