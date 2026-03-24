import React, { useState, useEffect } from 'react';
import { Transacao, CriarTransacao, Pessoa, Categoria, TipoTransacao, FinalidadeCategoria } from '../types';
import api from '../services/api';

const GerenciamentoTransacoes: React.FC = () => {
    const [transacoes, setTransacoes] = useState<Transacao[]>([]);
    const [pessoas, setPessoas] = useState<Pessoa[]>([]);
    const [categorias, setCategorias] = useState<Categoria[]>([]);
    const [categoriasFiltradas, setCategoriasFiltradas] = useState<Categoria[]>([]);
    const [dadosFormulario, setDadosFormulario] = useState<CriarTransacao>({
        descricao: '',
        valor: 0,
        tipo: TipoTransacao.Despesa,
        categoriaId: '',
        pessoaId: ''
    });
    const [pessoaSelecionada, setPessoaSelecionada] = useState<Pessoa | null>(null);
    const [erro, setErro] = useState<string>('');
    const [sucesso, setSucesso] = useState<string>('');
    const [filtroTipo, setFiltroTipo] = useState<string>('all');
    const [termoBusca, setTermoBusca] = useState<string>('');

    useEffect(() => {
        carregarDados();
    }, []);

    useEffect(() => {
        // Filtrar categorias com base no tipo de transação selecionado
        if (dadosFormulario.tipo !== undefined && categorias.length > 0) {
            const filtradas = categorias.filter(categoria => {
                if (dadosFormulario.tipo === TipoTransacao.Despesa) {
                    return categoria.finalidade === FinalidadeCategoria.Despesa || categoria.finalidade === FinalidadeCategoria.Ambos;
                } else {
                    return categoria.finalidade === FinalidadeCategoria.Receita || categoria.finalidade === FinalidadeCategoria.Ambos;
                }
            });
            setCategoriasFiltradas(filtradas);
            
            // Resetar seleção de categoria se a seleção atual não for válida
            if (dadosFormulario.categoriaId && !filtradas.some(c => c.id === dadosFormulario.categoriaId)) {
                setDadosFormulario(prev => ({ ...prev, categoriaId: '' }));
            }
        }
    }, [dadosFormulario.tipo, categorias]);

    const carregarDados = async () => {
        try {
            const [dadosTransacoes, dadosPessoas, dadosCategorias] = await Promise.all([
                api.obterTransacoes(),
                api.obterPessoas(),
                api.obterCategorias()
            ]);
            setTransacoes(dadosTransacoes);
            setPessoas(dadosPessoas);
            setCategorias(dadosCategorias);
        } catch (err) {
            setErro('Falha ao carregar dados');
            console.error(err);
        }
    };

    const handlePersonChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const pessoaId = e.target.value;
        const pessoa = pessoas.find(p => p.id === pessoaId) || null;
        setPessoaSelecionada(pessoa);
        setDadosFormulario({ ...dadosFormulario, pessoaId });
        
        // Se a pessoa for menor de idade, forçar tipo de transação para Despesa
        if (pessoa && pessoa.ehMenorDeIdade && dadosFormulario.tipo === TipoTransacao.Receita) {
            setDadosFormulario(prev => ({ ...prev, tipo: TipoTransacao.Despesa }));
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        
        if (name === 'valor') {
            const numValue = parseFloat(value);
            setDadosFormulario({
                ...dadosFormulario,
                [name]: isNaN(numValue) ? 0 : numValue
            });
        } else if (name === 'tipo') {
            const novoTipo = parseInt(value) as TipoTransacao;
            setDadosFormulario({ ...dadosFormulario, tipo: novoTipo, categoriaId: '' });
        } else {
            setDadosFormulario({ ...dadosFormulario, [name]: value });
        }
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
        if (dadosFormulario.valor <= 0) {
            setErro('Valor deve ser maior que 0');
            return;
        }
        if (!dadosFormulario.categoriaId) {
            setErro('Por favor, selecione uma categoria');
            return;
        }
        if (!dadosFormulario.pessoaId) {
            setErro('Por favor, selecione uma pessoa');
            return;
        }

        // Validação adicional para menores de idade
        if (pessoaSelecionada && pessoaSelecionada.ehMenorDeIdade && dadosFormulario.tipo === TipoTransacao.Receita) {
            setErro('Menores de idade só podem criar transações de despesa');
            return;
        }

        try {
            await api.criarTransacao(dadosFormulario);
            setSucesso('Transação criada com sucesso');
            resetarFormulario();
            await carregarDados();
            
            // Limpar mensagem de sucesso após 3 segundos
            setTimeout(() => setSucesso(''), 3000);
        } catch (err: any) {
            setErro(err.response?.data || 'Falha ao criar transação');
            console.error(err);
        }
    };

    const resetarFormulario = () => {
        setDadosFormulario({
            descricao: '',
            valor: 0,
            tipo: TipoTransacao.Despesa,
            categoriaId: '',
            pessoaId: ''
        });
        setPessoaSelecionada(null);
    };

    // Filtrar transações
    const transacoesFiltradas = transacoes.filter(transacao => {
        const matchesType = filtroTipo === 'all' || 
            (filtroTipo === 'expense' && transacao.tipo === TipoTransacao.Despesa) ||
            (filtroTipo === 'revenue' && transacao.tipo === TipoTransacao.Receita);
        
        const matchesSearch = transacao.descricao.toLowerCase().includes(termoBusca.toLowerCase()) ||
            transacao.nomePessoa.toLowerCase().includes(termoBusca.toLowerCase());
        
        return matchesType && matchesSearch;
    });

    return (
        <div className="bg-white shadow rounded-lg p-6">
            <h2 className="text-2xl font-bold mb-6">Gerenciamento de Transações</h2>
            
            {/* Formulário para criar transações */}
            <form onSubmit={handleSubmit} className="mb-8 p-4 bg-gray-50 rounded-lg">
                <h3 className="text-lg font-semibold mb-4">Criar Nova Transação</h3>
                
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
                        <textarea
                            name="descricao"
                            value={dadosFormulario.descricao}
                            onChange={handleInputChange}
                            maxLength={400}
                            rows={3}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                        />
                        <p className="text-xs text-gray-500 mt-1">
                            {dadosFormulario.descricao.length}/400 caracteres
                        </p>
                    </div>
                    
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Valor *
                        </label>
                        <input
                            type="number"
                            name="valor"
                            value={dadosFormulario.valor}
                            onChange={handleInputChange}
                            min="0.01"
                            step="0.01"
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                        />
                    </div>
                    
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Tipo *
                        </label>
                        <select
                            name="tipo"
                            value={dadosFormulario.tipo}
                            onChange={handleInputChange}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                            disabled={pessoaSelecionada?.ehMenorDeIdade || false}
                        >
                            <option value={TipoTransacao.Despesa}>Despesa</option>
                            <option value={TipoTransacao.Receita} disabled={pessoaSelecionada?.ehMenorDeIdade}>
                                Receita {pessoaSelecionada?.ehMenorDeIdade && '(Não disponível para menores)'}
                            </option>
                        </select>
                        {pessoaSelecionada?.ehMenorDeIdade && (
                            <p className="text-xs text-yellow-600 mt-1">
                                Menores de idade só podem criar transações de despesa
                            </p>
                        )}
                    </div>
                    
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Pessoa *
                        </label>
                        <select
                            name="pessoaId"
                            value={dadosFormulario.pessoaId}
                            onChange={handlePersonChange}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                        >
                            <option value="">Selecione uma pessoa</option>
                            {pessoas.map(pessoa => (
                                <option key={pessoa.id} value={pessoa.id}>
                                    {pessoa.nome} ({pessoa.idade} anos{pessoa.ehMenorDeIdade ? ' - Menor' : ''})
                                </option>
                            ))}
                        </select>
                    </div>
                    
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Categoria *
                        </label>
                        <select
                            name="categoriaId"
                            value={dadosFormulario.categoriaId}
                            onChange={handleInputChange}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            required
                        >
                            <option value="">Selecione uma categoria</option>
                            {categoriasFiltradas.map(categoria => (
                                <option key={categoria.id} value={categoria.id}>
                                    {categoria.descricao} ({categoria.exibirFinalidade})
                                </option>
                            ))}
                        </select>
                    </div>
                </div>
                
                <div className="mt-4">
                    <button
                        type="submit"
                        className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                        Criar Transação
                    </button>
                </div>
            </form>
            
            {/* Lista de transações com filtros */}
            <div>
                <div className="mb-4 flex flex-col sm:flex-row gap-4">
                    <div className="flex-1">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Filtrar por Tipo
                        </label>
                        <select
                            value={filtroTipo}
                            onChange={(e) => setFiltroTipo(e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        >
                            <option value="all">Todas as Transações</option>
                            <option value="expense">Apenas Despesas</option>
                            <option value="revenue">Apenas Receitas</option>
                        </select>
                    </div>
                    
                    <div className="flex-1">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Buscar
                        </label>
                        <input
                            type="text"
                            value={termoBusca}
                            onChange={(e) => setTermoBusca(e.target.value)}
                            placeholder="Buscar por descrição ou pessoa..."
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                </div>
                
                {transacoesFiltradas.length === 0 ? (
                    <p className="text-gray-500">Nenhuma transação encontrada.</p>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Data
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Descrição
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Valor
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Tipo
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Categoria
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Pessoa
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {transacoesFiltradas.map((transacao) => (
                                    <tr key={transacao.id}>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                            {new Date(transacao.criadoEm).toLocaleDateString('pt-BR')}
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="text-sm text-gray-900">{transacao.descricao}</div>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <div className={`text-sm font-semibold ${transacao.tipo === TipoTransacao.Receita ? 'text-green-600' : 'text-red-600'}`}>
                                                {transacao.tipo === TipoTransacao.Receita ? '+' : '-'} R$ {transacao.valor.toFixed(2)}
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${transacao.tipo === TipoTransacao.Receita ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                                                {transacao.exibirTipo}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                            {transacao.descricaoCategoria}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                            {transacao.nomePessoa}
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

export default GerenciamentoTransacoes;