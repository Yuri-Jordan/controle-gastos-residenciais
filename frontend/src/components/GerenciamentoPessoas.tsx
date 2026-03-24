import React, { useState, useEffect } from 'react';
import { Pessoa, CriarPessoa, AtualizarPessoa } from '../types';
import api from '../services/api';

const GerenciamentoPessoas: React.FC = () => {
    const [pessoas, setPessoas] = useState<Pessoa[]>([]);
    const [editando, setEditando] = useState(false);
    const [pessoaAtual, setPessoaAtual] = useState<Pessoa | null>(null);
    const [dadosFormulario, setDadosFormulario] = useState<CriarPessoa>({ nome: '', idade: 0 });
    const [erro, setErro] = useState<string>('');
    const [sucesso, setSucesso] = useState<string>('');

    // Carregar pessoas ao montar o componente
    useEffect(() => {
        carregarPessoas();
    }, []);

    const carregarPessoas = async () => {
        try {
            const dados = await api.obterPessoas();
            setPessoas(dados);
        } catch (err) {
            setErro('Falha ao carregar pessoas');
            console.error(err);
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setDadosFormulario({
            ...dadosFormulario,
            [name]: name === 'age' ? parseInt(value) || 0 : value
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setErro('');
        setSucesso('');

        // Validação
        if (!dadosFormulario.nome.trim()) {
            setErro('Nome é obrigatório');
            return;
        }
        if (dadosFormulario.nome.length > 200) {
            setErro('Nome não pode exceder 200 caracteres');
            return;
        }
        if (dadosFormulario.idade < 0) {
            setErro('Idade deve ser maior ou igual a 0');
            return;
        }

        try {
            if (editando && pessoaAtual) {
                // Atualizar pessoa existente
                const dadosAtualizacao: AtualizarPessoa = {
                    nome: dadosFormulario.nome,
                    idade: dadosFormulario.idade
                };
                await api.atualizarPessoa(pessoaAtual.id, dadosAtualizacao);
                setSucesso('Pessoa atualizada com sucesso');
            } else {
                // Criar nova pessoa
                await api.criarPessoa(dadosFormulario);
                setSucesso('Pessoa criada com sucesso');
            }
            
            // Resetar formulário e recarregar dados
            resetarFormulario();
            await carregarPessoas();
        } catch (err: any) {
            setErro(err.response?.data || 'Falha ao salvar pessoa');
            console.error(err);
        }
    };

    const handleEdit = (pessoa: Pessoa) => {
        setEditando(true);
        setPessoaAtual(pessoa);
        setDadosFormulario({
            nome: pessoa.nome,  
            idade: pessoa.idade
        });
        setErro('');
        setSucesso('');
    };

    const handleDelete = async (id: string) => {
        if (window.confirm('Tem certeza que deseja excluir esta pessoa? Isso também excluirá todas as transações associadas.')) {
            try {
                await api.excluirPessoa(id);
                setSucesso('Pessoa excluída com sucesso');
                await carregarPessoas();
            } catch (err: any) {
                setErro(err.response?.data || 'Falha ao excluir pessoa');
                console.error(err);
            }
        }
    };

    const resetarFormulario = () => {
        setEditando(false);
        setPessoaAtual(null);
        setDadosFormulario({ nome: '', idade: 0 });
        setTimeout(() => {
            setErro('');
            setSucesso('');
        }, 3000);
    };

    return (
        <div className="bg-white shadow rounded-lg p-6">
            <h2 className="text-2xl font-bold mb-6">Gerenciamento de Pessoas</h2>
            
            {/* Formulário para criar/editar pessoas */}
            <form onSubmit={handleSubmit} className="mb-8 p-4 bg-gray-50 rounded-lg">
                <h3 className="text-lg font-semibold mb-4">
                    {editando ? 'Editar Pessoa' : 'Criar Nova Pessoa'}
                </h3>
                
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
                            Nome *
                        </label>
                        <input
                            type="text"
                            name="nome"
                            value={dadosFormulario.nome}
                            onChange={handleInputChange}
                            maxLength={200}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                        />
                        <p className="text-xs text-gray-500 mt-1">
                            {dadosFormulario.nome.length}/200 caracteres
                        </p>
                    </div>
                    
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Idade *
                        </label>
                        <input
                            type="number"
                            name="idade"
                            value={dadosFormulario.idade}
                            onChange={handleInputChange}
                            min="0"
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                        />
                    </div>
                </div>
                
                <div className="mt-4 flex space-x-2">
                    <button
                        type="submit"
                        className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                        {editando ? 'Atualizar' : 'Criar'}
                    </button>
                    {editando && (
                        <button
                            type="button"
                            onClick={resetarFormulario}
                            className="px-4 py-2 bg-gray-300 text-gray-700 rounded-md hover:bg-gray-400 focus:outline-none focus:ring-2 focus:ring-gray-500"
                        >
                            Cancelar
                        </button>
                    )}
                </div>
            </form>
            
            {/* Lista de pessoas */}
            <div>
                <h3 className="text-lg font-semibold mb-4">Lista de Pessoas</h3>
                {pessoas.length === 0 ? (
                    <p className="text-gray-500">Nenhuma pessoa encontrada.</p>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Nome
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Idade
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Status
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Ações
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {pessoas.map((pessoa) => (
                                    <tr key={pessoa.id}>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            {pessoa.nome}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            {pessoa.idade}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            {pessoa.ehMenorDeIdade ? (
                                                <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-yellow-100 text-yellow-800">
                                                    Menor
                                                </span>
                                            ) : (
                                                <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                                                    Adulto
                                                </span>
                                            )}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap space-x-2">
                                            <button
                                                onClick={() => handleEdit(pessoa)}
                                                className="text-blue-600 hover:text-blue-900"
                                            >
                                                Editar
                                            </button>
                                            <button
                                                onClick={() => handleDelete(pessoa.id)}
                                                className="text-red-600 hover:text-red-900"
                                            >
                                                Excluir
                                            </button>
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

export default GerenciamentoPessoas;